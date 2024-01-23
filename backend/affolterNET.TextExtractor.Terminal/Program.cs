using System.Reflection;
using affolterNET.TextExtractor.Core;
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Terminal;
using affolterNET.TextExtractor.Terminal.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Extensions.Hosting;

try
{
    await Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(cfg =>
        {
            cfg.AddUserSecrets<Program>();
            cfg.AddJsonFile("appsettings.json", optional: true);
            cfg.AddEnvironmentVariables();
            cfg.AddCommandLine(args,new Dictionary<string, string> {
                { "-c", "POSTGRES_CONNSTRING" },
                { "--connectionstring", "POSTGRES_CONNSTRING" },
                { "-e", "FedlexSparqlEndpoint" },
                { "--endpoint", "FedlexSparqlEndpoint" },
                { "-r", "AWS_REGION" },
                { "--awsregion", "AWS_REGION" },
                { "-b", "AWS_BUCKET" },
                { "--awsbucket", "AWS_BUCKET" },
                { "-k", "AWS_KEY" },
                { "--awskey", "AWS_KEY" },
                { "-s", "AWS_SECRET" },
                { "--awssecret", "AWS_SECRET" },
                { "-u", "AWS_SQSURL" },
                { "--awssqsurl", "AWS_SQSURL" },
                { "-l", "LogLevel" },
                { "--loglevel", "LogLevel" },
            });
        })
        .ConfigureServices((ctx, services) =>
        {
            var logLevel = ctx.Configuration.GetAndCheckValue("LogLevel");
            EnumLogLevel enmLogLevel = EnumLogLevel.Info;
            var vals = Enum.GetValues<EnumLogLevel>();
            foreach (var v in vals)
            {
                if (v.ToString().ToLower() == logLevel.ToLower())
                {
                    enmLogLevel = v;
                    break;
                }

                if (((int)v).ToString() == logLevel)
                {
                    enmLogLevel = v;
                    break;
                }
            }
            services.AddTransient<AnsiConsoleWrapper>(_ => new AnsiConsoleWrapper(enmLogLevel));
            services.AddTransient<IOutput, AnsiConsoleOutputter>();
            services.AddCoreServices(ctx.Configuration);
            services.AddTransient<DetectTextCommand>();
        })
        .UseConsoleLifetime()
        .UseSpectreConsole<DetectTextCommand>(config =>
        {
            config.PropagateExceptions();

            const string lawsAlias = "parse";
            config.AddCommand<DetectTextCommand>("parse-pdf")
                .WithAlias(lawsAlias)
                .WithDescription("get text and textblocks from pdf files")
                .WithExample($"dotnet {Assembly.GetExecutingAssembly().GetName().Name}.dll", lawsAlias,
                    "[-f|--file <FILE-PATH>]");
        })
        .RunConsoleAsync();
    return Environment.ExitCode;
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    return -1;
}
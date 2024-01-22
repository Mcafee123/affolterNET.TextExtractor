using System.Reflection;
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
            var connString = ctx.Configuration.GetAndCheckValue("POSTGRES_CONNSTRING");
            var fedlexSparqlEndpoint = ctx.Configuration.GetAndCheckValue("FedlexSparqlEndpoint");
            var awsRegion = ctx.Configuration.GetAndCheckValue("AWS_REGION");
            var awsBucket = ctx.Configuration.GetAndCheckValue("AWS_REGION");
            var awsKey = ctx.Configuration.GetAndCheckValue("AWS_KEY");
            var awsSecret = ctx.Configuration.GetAndCheckValue("AWS_SECRET");
            var awsSqsUrl = ctx.Configuration.GetAndCheckValue("AWS_SQSURL");
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
            // services.AddSingleton(new FedlexLawsInForceCommand.Settings(connString, fedlexSparqlEndpoint));
            // services.AddSingleton(new FedlexDownloadFilesCommand.Settings(connString, awsRegion, awsBucket, awsKey, awsSecret, awsSqsUrl));
            // services.AddSingleton(new FedlexParsePdfCommand.Settings());
            // services.ConfigureDb(connString);
            // services.AddTransient<AnsiConsoleWrapper>(_ => new AnsiConsoleWrapper(enmLogLevel));
            // services.AddTransient<IOutput, AnsiConsoleOutputter>();
            //
            // services.AddCoreServices(ctx.Configuration);
            // services.AddPdfServices();
        })
        .UseConsoleLifetime()
        .UseSpectreConsole<DetectTextCommand>(config =>
        {
            config.PropagateExceptions();

            const string lawsAlias = "laws";
            config.AddCommand<FedlexLawsInForceCommand>("fedlex-laws")
                .WithAlias(lawsAlias)
                .WithDescription("Get laws in force from SPARQL-Endpoint and save them to the DB")
                .WithExample($"dotnet {Assembly.GetExecutingAssembly().GetName().Name}.dll", lawsAlias,
                    "[-c|--connectionstring <POSTGRES-CONNSTRING>]", "[-e|--endpoint <SPARQL-ENDPOINT>]");
            
            const string dwnldAlias = "download";
            config.AddCommand<FedlexDownloadFilesCommand>("fedlex-download")
                .WithAlias(dwnldAlias)
                .WithDescription("download files and put them on S3 and into the DB")
                .WithExample($"dotnet {Assembly.GetExecutingAssembly().GetName().Name}.dll", dwnldAlias,
                    "[-c|--connectionstring <POSTGRES-CONNSTRING>]");
            
            const string pdfAlias = "pdf";
            config.AddCommand<FedlexParsePdfCommand>("fedlex-pdf")
                .WithAlias(pdfAlias)
                .WithDescription("parse pdf-files")
                .WithExample($"dotnet {Assembly.GetExecutingAssembly().GetName().Name}.dll", pdfAlias,
                    "[-c|--connectionstring <POSTGRES-CONNSTRING>]");
        })
        .RunConsoleAsync();
    return Environment.ExitCode;
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    return -1;
}
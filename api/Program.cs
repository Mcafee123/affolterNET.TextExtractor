
using System.Text.Json;
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Storage.Extensions;
using api.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
        {
            jsonSerializerOptions.ConfigureJsonSerializerOptions();
        });
    })
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddUserSecrets<Program>();
    })
    .ConfigureServices((ctx, services) =>
    {
        // Add ApplicationInsights services for non-HTTP applications.
        // See https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service and
        // See https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#application-insights
        services.AddApplicationInsightsTelemetryWorkerService();

        // Add function app specific ApplicationInsights services.
        // See https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#application-insights
        services.ConfigureFunctionsApplicationInsights();
        
        services.AddTransient<IOutput, FunctionsLogger>();
        services.AddTextExtractorCoreServices(ctx.Configuration);
        services.AddTextExtractorStorageServices(ctx.Configuration);
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        // see the logger category-names:
        // logging.AddJsonConsole(options =>
        // {
        //     options.JsonWriterOptions = new JsonWriterOptions() { Indented = true };
        // });
        
        // Make sure the configuration of the appsettings.json file is picked up.
        // var config = new ConfigurationBuilder()
        //     .AddJsonFile("host.json")
        //     .Build()
        //     .GetSection("Logging");
        // logging.AddConfiguration(config.GetSection("Logging"));
        // logging.AddConsole();
            
        // You will need extra configuration because above will only log per default Warning (default AI configuration) and above because of following line:
        // https://github.com/microsoft/ApplicationInsights-dotnet/blob/main/NETCORE/src/Shared/Extensions/ApplicationInsightsExtensions.cs#L427
        // This is documented here:
        // https://github.com/microsoft/ApplicationInsights-dotnet/issues/2610#issuecomment-1316672650
        // So remove the default logger rule (warning and above). This will result that the default will be Information.
        // logging.Services.Configure<LoggerFilterOptions>(options =>
        // {
        //     var toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
        //                                                         == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
        //     if (toRemove is not null)
        //     {
        //         options.Rules.Remove(toRemove);
        //     }
        // });
        
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .Build();

await host.RunAsync();
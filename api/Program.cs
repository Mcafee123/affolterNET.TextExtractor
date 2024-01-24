using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using api.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((ctx, services) =>
    {
        services.AddTransient<IOutput, FunctionsLogger>();
        services.AddCoreServices(ctx.Configuration);
        // services.AddApplicationInsightsTelemetryWorkerService();
        // services.ConfigureFunctionsApplicationInsights();
        // services.AddSingleton<IHttpResponderService, DefaultHttpResponderService>();
        // s.Configure<LoggerFilterOptions>(options =>
        // {
        //     // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
        //     // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
        //     LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
        //                                                                      == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
        //
        //     if (toRemove is not null)
        //     {
        //         options.Rules.Remove(toRemove);
        //     }
        // });
    })
    .Build();

await host.RunAsync();
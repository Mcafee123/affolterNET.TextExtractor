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
        services.AddTextExtractorCoreServices(ctx.Configuration);
    })
    .Build();

await host.RunAsync();
using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UglyToad.PdfPig.Util;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class ConfigureServicesExtension
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IWordExtractor, PdfWordExtractor>();
        services.AddTransient<ILineDetector, LineDetector>();
        services.AddTransient<IBlockDetector, BlockDetector>();
        services.AddTransient<ReadWordsStep>();
        services.AddTransient<DetectLinesStep>();
        services.AddTransient<DetectTextBlocksStep>();
        services.AddTransient<BasicProcessingPipeline>();
    }
}
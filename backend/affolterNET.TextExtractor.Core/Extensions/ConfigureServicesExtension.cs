using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services;
using affolterNET.TextExtractor.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UglyToad.PdfPig.Util;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class ConfigureServicesExtension
{
    public static void AddTextExtractorCoreServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IWordExtractor, PdfWordExtractor>();
        services.AddTransient<IWordCleaner, WordCleaner>();
        services.AddTransient<ILineDetector, LineDetector>();
        services.AddTransient<IBlockDetector, BlockDetector>();
        services.AddTransient<IFootnoteDetector, FootnoteDetector>();
        services.AddTransient<ReadPagesStep>();
        services.AddTransient<CleanWordsStep>();
        services.AddTransient<DetectLinesStep>();
        services.AddTransient<AnalyzeLineSpacingStep>();
        services.AddTransient<DetectFootnotesStep>();
        services.AddTransient<DetectTextBlocksStep>();
        services.AddTransient<BasicPdfPipeline>();
    }
}
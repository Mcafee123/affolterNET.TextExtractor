using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services;
using affolterNET.TextExtractor.Core.Services.Detectors;
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
        services.AddTransient<IPageNumberService, PageNumberService>();
        services.AddTransient<IHeaderService, HeaderService>();
        services.AddTransient<IFixSpacesService, FixSpacesService>();
        services.AddTransient<ReadPagesStep>();
        services.AddTransient<CleanWordsStep>();
        services.AddTransient<AnalyzeLineSpacingStep>();
        services.AddTransient<DetectFootnotesStep>();
        services.AddTransient<DetectTextBlocksStep>();
        services.AddTransient<DetectPageNumberStep>();
        services.AddTransient<DetectHeadersStep>();
        services.AddTransient<FixSpacesStep>();
        services.AddTransient<ExtractTextStep>();
        services.AddTransient<SerializeToBlobStorageStep>();
        services.AddTransient<IBasicPdfPipeline, BasicPdfPipeline>();

        services.AddSingleton<LoggerContextProvider>();
        services.AddSingleton<ServiceFactory>();
    }
}
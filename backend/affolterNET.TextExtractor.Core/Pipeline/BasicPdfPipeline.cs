using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;

namespace affolterNET.TextExtractor.Core.Pipeline;

public class BasicPdfPipeline : IBasicPdfPipeline
{
    private readonly IOutput _log;
    private readonly ProcessingPipeline _pipeline;

    public BasicPdfPipeline(ReadPagesStep readPagesStep, CleanWordsStep cleanWordsStep,
        AnalyzeLineSpacingStep analyzeLineSpacingStep, DetectTextBlocksStep detectBlocksStep,
        DetectFootnotesStep detectFootnotesStep, DetectPageNumberStep detectPageNumberStep, 
        DetectHeadersStep detectHeadersStep, FixSpacesStep fixSpacesStep, 
        ExtractTextStep extractTextStep, IOutput log)
    {
        _log = log;
        _pipeline = new ProcessingPipeline();
        _pipeline.AddStep(readPagesStep);
        _pipeline.AddStep(cleanWordsStep);
        _pipeline.AddStep(detectBlocksStep);
        _pipeline.AddStep(analyzeLineSpacingStep);
        _pipeline.AddStep(detectPageNumberStep);
        _pipeline.AddStep(detectHeadersStep);
        _pipeline.AddStep(detectFootnotesStep);
        _pipeline.AddStep(fixSpacesStep);
        // _pipeline.AddStep(extractTextStep);
    }
    
    public async Task Execute(IPipelineContext context)
    {
        await Task.Run(() => _pipeline.ExecutePipeline(context));
        _log.Write(EnumLogLevel.Warning, "[blue]", "Pipeline finished", "[/]");
    }

    public void AddStep(IPipelineStep step)
    {
        _pipeline.AddStep(step);
    }
}
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;

namespace affolterNET.TextExtractor.Core.Pipeline;

public class BasicPdfPipeline : IBasicPdfPipeline
{
    private readonly IOutput _log;
    private readonly ProcessingPipeline _pipeline;

    public BasicPdfPipeline(ReadPagesStep readPagesStep, CleanWordsStep cleanWordsStep, DetectLinesStep detectLinesStep,
        AnalyzeLineSpacingStep analyzeLineSpacingStep, DetectTextBlocksStep detectBlocksStep,
        DetectFootnotesStep detectFootnotesStep, ExtractTextStep extractTextStep, IOutput log)
    {
        _log = log;
        _pipeline = new ProcessingPipeline();
        _pipeline.AddStep(readPagesStep);
        _pipeline.AddStep(cleanWordsStep);
        _pipeline.AddStep(detectLinesStep);
        _pipeline.AddStep(analyzeLineSpacingStep);
        _pipeline.AddStep(detectBlocksStep);
        _pipeline.AddStep(detectFootnotesStep);
        // _pipeline.AddStep(extractTextStep);
    }

    public void Execute(IPipelineContext context)
    {
        _pipeline.ExecutePipeline(context);
        _log.Write(EnumLogLevel.Warning, "[blue]", "Pipeline finished", "[/]");
    }

    public void AddStep(IPipelineStep step)
    {
        _pipeline.AddStep(step);
    }
}
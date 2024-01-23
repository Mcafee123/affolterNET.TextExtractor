using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;

namespace affolterNET.TextExtractor.Core.Pipeline;

public class BasicProcessingPipeline
{
    private readonly ReadWordsStep _readWordsStep;
    private readonly DetectLinesStep _detectLinesStep;
    private readonly DetectTextBlocksStep _detectBlocksStep;
    private readonly IOutput _log;
    private readonly ProcessingPipeline _pipeline;

    public BasicProcessingPipeline(ReadWordsStep readWordsStep, DetectLinesStep detectLinesStep,
        DetectTextBlocksStep detectBlocksStep, IOutput log)
    {
        _readWordsStep = readWordsStep;
        _detectLinesStep = detectLinesStep;
        _detectBlocksStep = detectBlocksStep;
        _log = log;
        _pipeline = new ProcessingPipeline();
    }

    public void Execute(IPipelineContext context)
    {
        BuildPipeline();
        _pipeline.ExecutePipeline(context);
        _log.Write(EnumLogLevel.Info, "[blue]", "Pipeline finished", "[/]");
    }

    private void BuildPipeline()
    {
        _pipeline.AddStep(_readWordsStep);
        _pipeline.AddStep(_detectLinesStep);
        _pipeline.AddStep(_detectBlocksStep);
    }
}
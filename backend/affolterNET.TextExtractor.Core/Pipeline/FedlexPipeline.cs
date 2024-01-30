using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;

namespace affolterNET.TextExtractor.Core.Pipeline;

public class FedlexPipeline
{
    private readonly ReadWordsStep _readWordsStep;
    private readonly CleanWordsStep _cleanWordsStep;
    private readonly DetectLinesStep _detectLinesStep;
    private readonly DetectTextBlocksStep _detectBlocksStep;
    private readonly IOutput _log;
    private readonly ProcessingPipeline _pipeline;

    public FedlexPipeline(ReadWordsStep readWordsStep, CleanWordsStep cleanWordsStep, DetectLinesStep detectLinesStep,
        DetectTextBlocksStep detectBlocksStep, IOutput log)
    {
        _readWordsStep = readWordsStep;
        _cleanWordsStep = cleanWordsStep;
        _detectLinesStep = detectLinesStep;
        _detectBlocksStep = detectBlocksStep;
        _log = log;
        _pipeline = new ProcessingPipeline();
    }

    public void Execute(IFedlexPipelineContext context)
    {
        BuildPipeline();
        _pipeline.ExecutePipeline(context);
        _log.Write(EnumLogLevel.Info, "[blue]", "Pipeline finished", "[/]");
    }

    private void BuildPipeline()
    {
        _pipeline.AddStep(_readWordsStep);
        _pipeline.AddStep(_cleanWordsStep);
        _pipeline.AddStep(_detectLinesStep);
        _pipeline.AddStep(_detectBlocksStep);
    }
}
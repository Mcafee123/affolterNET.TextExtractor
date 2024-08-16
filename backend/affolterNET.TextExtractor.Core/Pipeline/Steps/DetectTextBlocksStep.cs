using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class DetectTextBlocksStep: IPipelineStep
{
    private readonly IBlockDetector _blockDetector;
    private readonly IOutput _log;

    public DetectTextBlocksStep(IBlockDetector blockDetector, IOutput log)
    {
        _blockDetector = blockDetector;
        _log = log;
    }
    public class DetectTextBlocksStepSettings: IStepSettings
    {
        public double VerticalBlockDistanceDiffFactor { get; set; } = 1.8;
        public double BlockOverlapDistanceDiff { get; set; } = 0.4;
        public double QuadtreeBlockResolution { get; set; } = 3;
        public double BaseLineMatchingRange { get; set; } = 0.2;
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Detecting textblocks");
        var settings = context.GetSettings<DetectTextBlocksStepSettings>();
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }

        var blockCount = 0;
        var blockCountWithHorizontal = 0;
        foreach (var page in context.Document.Pages)
        {
            // find blocks
            _blockDetector.FindBlocks(
                page,
                settings.VerticalBlockDistanceDiffFactor,
                settings.BlockOverlapDistanceDiff,
                settings.BaseLineMatchingRange,
                settings.QuadtreeBlockResolution);
            blockCount += page.Blocks.Count;
        }

        _log.Write(EnumLogLevel.Debug, $"Blocks: {blockCount}");
        _log.Write(EnumLogLevel.Debug, $"Blocks (incl. horizontal): {blockCountWithHorizontal}");
    }
}
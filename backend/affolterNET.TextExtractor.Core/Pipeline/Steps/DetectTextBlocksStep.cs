using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services;
using UglyToad.PdfPig.DocumentLayoutAnalysis;

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
    public void Execute(IPipelineContext context)
    {
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadWordsStep)} before this step");
        }

        var blockCount = 0;
        var blockCountWithHorizontal = 0;
        foreach (var page in context.Document.Pages)
        {
            page.Blocks.Clear();
            
            // // one block per page
            // var tb = new PdfTextBlock();
            // tb.AddLines(page.Lines);
            // page.Blocks.Add(tb);
            // // one block per page
            
            // find blocks by connecting lines
            var blocks = _blockDetector.FindBlocks(page);
            blockCount += blocks.Count;
            // blocks = _blockDetector.FindHorizontalBlocks(blocks);
            // blockCountWithHorizontal += blocks.Count;
            page.Blocks.AddRange(blocks.ToList());
            // find blocks by connecting lines
        }
        _log.Write(EnumLogLevel.Debug, "[yellow]", $"Blocks: {blockCount}", "[/]");
        _log.Write(EnumLogLevel.Debug, "[yellow]", $"Blocks (incl. horizontal): {blockCountWithHorizontal}", "[/]");
    }
}
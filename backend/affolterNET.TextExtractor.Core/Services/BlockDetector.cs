using System.Diagnostics;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Services;

public class BlockDetector : IBlockDetector
{
    private readonly ILineDetector _lineDetector;
    private readonly IOutput _log;

    public BlockDetector(ILineDetector lineDetector, IOutput log)
    {
        _lineDetector = lineDetector;
        _log = log;
    }

    public FontSizeSettings FontSizes { get; private set; } = new([]);
    public double NewBlockDistDiff { get; private set; }
    public double BaseLineMatchingRange { get; private set; }

    public void FindBlocks(IPdfPage page, FontSizeSettings fontSizeSettings, double horizontalDistDiff,
        double blockOverlapDistanceDiff, double baseLineMatchingRange, double quadtreeBlockResolution)
    {
        // block list
        var blockList = new List<IPdfTextBlock>();
        
        // first, one block per page
        var mainPageBlock = new PdfTextBlock(page);
        mainPageBlock.SetLines(page.Lines);
        blockList.Add(mainPageBlock);
        
        // create quadtree
        var tree = new Quadtree(mainPageBlock.BoundingBox, quadtreeBlockResolution);
        foreach (var word in mainPageBlock.Words)
        {
            tree.Insert(word.BoundingBox);
        }
        
        // get settings
        FontSizes = fontSizeSettings;
        NewBlockDistDiff = horizontalDistDiff;
        BaseLineMatchingRange = baseLineMatchingRange;

        // separate into smaller blocks in a loop.
        // stop if nothing changes anymore
        var runs = 0;
        int lastBlockCount;
        var sw = new Stopwatch();
        sw.Start();
        do
        {
            // last block count
            lastBlockCount = blockList.Count;
            // detect gaps and fix blocks
            var toRemove = new List<IPdfTextBlock>();
            var toAdd = new List<IPdfTextBlock>();
            foreach (var block in blockList)
            {
                if (block.Words.Count < 2)
                {
                    // no need to separate blocks with less than 2 words
                    continue;
                }
                if (block.FontSizeAvg / 2 < tree.DeepestLevelNodeHeight)
                {
                    // for blocks with small text, use a quadtree with lower resolution
                    var smallTree = new Quadtree(block.BoundingBox, block.FontSizeAvg / 2);
                    foreach (var word in block.Words)
                    {
                        smallTree.Insert(word.BoundingBox);
                    }
                    block.BlockNodes = smallTree.GetRowsAndColumns(block.BoundingBox);
                    block.HorizontalGaps = smallTree.GetHorizontalGaps(block.BlockNodes);
                    block.VerticalGaps = smallTree.GetVerticalGaps(block.BlockNodes);
                }
                else
                {
                    // main resolution is ok
                    block.BlockNodes = tree.GetRowsAndColumns(block.BoundingBox);
                    block.HorizontalGaps = tree.GetHorizontalGaps(block.BlockNodes);
                    block.VerticalGaps = tree.GetVerticalGaps(block.BlockNodes);
                }
                var newBlocks = SeparateBlocks(block);
                if (newBlocks.Count > 0)
                {
                    toRemove.Add(block);
                    toAdd.AddRange(newBlocks);
                }
            }

            foreach (var rem in toRemove)
            {
                blockList.Remove(rem);
            }

            blockList.AddRange(toAdd);
            runs++;
        } while (lastBlockCount != blockList.Count);

        _log.Write(EnumLogLevel.Debug, $"Detected {blockList.Count} Blocks in {runs} runs. Duration: {sw.ElapsedMilliseconds} ms");
        page.Blocks.AddRange(blockList);
    }

    private List<IPdfTextBlock> SeparateBlocks(IPdfTextBlock block)
    {
        var horizontalBlocks = GetHorizontalBlocks(block);
        if (horizontalBlocks.Count > 0)
        {
            return horizontalBlocks;
        }

        var verticalBlocks = GetVerticalBlocks(block);
        if (verticalBlocks.Count > 0)
        {
            return verticalBlocks;
        }

        return new List<IPdfTextBlock>();
    }
    
    private List<IPdfTextBlock> GetHorizontalBlocks(IPdfTextBlock block)
    {
        var blocksAdded = false;
        var blockWords = new List<IWordOnPage>();
        blockWords.AddRange(block.Words);
        var horizontalBlocks = new List<IPdfTextBlock>();
        PdfRectangle? lastGap = null;
        var newBlock = new PdfTextBlock(block.Page);
        horizontalBlocks.Add(newBlock);
        foreach (var gap in block.HorizontalGaps)
        {
            // add the words above the gap
            var upperWords = blockWords.Where(w => w.BoundingBox.Bottom >= gap.Top).ToList();
            if (upperWords.Count < 1)
            {
                continue;
            }

            // remove words from original block
            WordsToBlock(blockWords, newBlock, upperWords);

            // remember the last gap
            lastGap = gap;

            // // get the line above the gap
            // var upperLines = block.Lines.Where(l => l.BoundingBox.Centroid.Y > gap.Top).ToList();
            // if (!upperLines.Any())
            // {
            //     continue;
            // }
            //
            // var line = upperLines.MinBy(l => l.BoundingBox.Bottom);
            // if (line == null)
            // {
            //     continue;
            // }
            //
            // // check if gap is bigger than the common line spacing
            // var spacing = FontSizes.GetCommonLineSpacing(line.FontSizeAvg);
            // // if topdistance is bigger, add new block
            // if (gap.Height > spacing)
            // {
                blocksAdded = true;
                newBlock = new PdfTextBlock(block.Page);
                horizontalBlocks.Add(newBlock);
            //}
        }

        if (lastGap != null)
        {
            var lowerWords = blockWords.Where(w => w.BoundingBox.Top <= lastGap.Value.Bottom).ToList();
            if (lowerWords.Count > 0)
            {
                WordsToBlock(blockWords, newBlock, lowerWords);
            }
        }

        if (blocksAdded)
        {
            if (blockWords.Count != 0)
            {
                throw new InvalidOperationException("BlockDetector: Not all words were assigned to a new block.");
            }

            return horizontalBlocks;
        }

        return new List<IPdfTextBlock>();
    }

    private List<IPdfTextBlock> GetVerticalBlocks(IPdfTextBlock block)
    {
        var blocksAdded = false;
        var blockWords = new List<IWordOnPage>();
        blockWords.AddRange(block.Words);
        var verticalBlocks = new List<IPdfTextBlock>();
        PdfRectangle? lastGap = null;
        var newBlock = new PdfTextBlock(block.Page);
        verticalBlocks.Add(newBlock);
        foreach (var gap in block.VerticalGaps)
        {
            // add the words left of the gap
            var leftWords = blockWords.Where(w => w.BoundingBox.Right <= gap.Left).ToList();
            if (leftWords.Count < 1)
            {
                continue;
            }

            // remove words from original block
            WordsToBlock(blockWords, newBlock, leftWords);

            // remember the last gap
            lastGap = gap;

            var verticalDistDiff = 2 * (newBlock.Lines.WordSpaceAvg ?? 0);
            if (gap.Width > verticalDistDiff + NewBlockDistDiff)
            {
                blocksAdded = true;
                newBlock = new PdfTextBlock(block.Page);
                verticalBlocks.Add(newBlock);
            }
        }
        
        if (lastGap != null)
        {
            var rightWords = blockWords.Where(w => w.BoundingBox.Left >= lastGap.Value.Right).ToList();
            if (rightWords.Count > 0)
            {
                WordsToBlock(blockWords, newBlock, rightWords);
            }
        }
        
        if (blocksAdded)
        {
            if (blockWords.Count != 0)
            {
                throw new InvalidOperationException("BlockDetector: Not all words were assigned to a new block.");
            }

            return verticalBlocks;
        }

        return new List<IPdfTextBlock>();
    }

    private void WordsToBlock(List<IWordOnPage> blockWords, IPdfTextBlock newBlock, List<IWordOnPage> words)
    {
        foreach (var word in words)
        {
            blockWords.Remove(word);
        }

        var lines = _lineDetector.DetectLines(words, BaseLineMatchingRange);
        newBlock.AddLines(lines.ToList());
    }
}
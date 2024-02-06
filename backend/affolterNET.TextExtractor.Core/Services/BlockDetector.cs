using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Services;

public class BlockDetector: IBlockDetector
{
    private readonly IOutput _log;

    public BlockDetector(IOutput log)
    {
        _log = log;
    }

    public IPdfTextBlocks FindBlocks(IPdfPage page, FontSizeSettings fontSizeSettings, double newBlockDistanceDiff, double blockOverlapDistanceDiff)
    {
        var lines = page.Lines;
        var innerBlocks = new PdfTextBlocks();
        var allLines = new PdfLines();
        allLines.AddRange(lines.ToList());

        // loop through lines top down
        foreach (var line in lines)
        {
            // if an already added block overlaps the current line, add to this block later
            var existingOverlappingY = innerBlocks
                .FirstOrDefault(b => b.BoundingBox.Overlaps(line.BoundingBox, blockOverlapDistanceDiff));
            if (existingOverlappingY == null)
            {
                // if there is no overlapping block, check the distance to the next line on top
                var addBlock = line.TopDistance >= LineOnPage.DefaultTopDistance;
                // if top distance is LineOnPage.DefaultTopDistance, add block
                if (!addBlock)
                {
                    // check if line.TopDistance is bigger than the common line spacing
                    var distanceDiff = fontSizeSettings.GetTopDistanceDiff(line.FontSizeAvg, line.TopDistance);
                    // if topdistance is bigger, add block
                    addBlock = distanceDiff >= newBlockDistanceDiff;
                }
                
                if (addBlock)
                {
                    var tb = new PdfTextBlock();
                    tb.AddLine(line);
                    innerBlocks.Add(tb);
                    allLines.Remove(line);
                }
            }
        }
        
        // for lines with small distances, append to existing blocks
        foreach (var currentLine in allLines)
        {
            var blockToAppend = innerBlocks.FirstOrDefault(b => b.BoundingBox.Overlaps(currentLine.BoundingBox, blockOverlapDistanceDiff));
            if (blockToAppend == null)
            {
                var nextLineOnTop = page.Lines.FindLineOnTop(currentLine);
                blockToAppend = innerBlocks.FirstOrDefault(b => b.Any(l => l == nextLineOnTop));
            }
            
            if (blockToAppend == null)
            {
                throw new InvalidOperationException($"block to append not found: {currentLine}");
            }
            
            blockToAppend.AddLine(currentLine);
        }
        
        // fix blocks that are near enough by text size
        var blocksToRemove = new List<IPdfTextBlock>();
        foreach (var b in innerBlocks)
        {
            // find the nearest block on top, that overlaps
            var blockOnTop = innerBlocks
                .Where(bl => bl.BoundingBox.OverlapsX(b.BoundingBox)
                    && bl.BoundingBox.Bottom > b.BoundingBox.Top)
                .MinBy(bl => bl.BoundingBox.Bottom);
            if (blockOnTop != null)
            {
                var firstLine = b.FirstLine!;
                var firstLineGroup = fontSizeSettings.GetGroup(firstLine.FontSizeAvg);
                // get the lowest line in the block on top, that overlaps the first line of the current block
                // and fontsize is in the same font-size-group
                var lowestLine = blockOnTop.Lines
                    .Where(l => l.BoundingBox.OverlapsX(firstLine.BoundingBox))
                    .MinBy(l => l.BaseLineY);
                if (lowestLine == null)
                {
                    continue;
                }
                var lowestLineGroup = fontSizeSettings.GetGroup(lowestLine.FontSizeAvg);
                if (lowestLineGroup.GroupId != firstLineGroup.GroupId)
                {
                    continue;
                }

                var dist = firstLine.GetTopDistance(lowestLine);
                var distanceDiff = fontSizeSettings.GetTopDistanceDiff(firstLine.FontSizeAvg, dist);
                if (distanceDiff < newBlockDistanceDiff)
                {
                    blockOnTop.AddLines(b.Lines.ToList());
                    blocksToRemove.Add(b);
                }
            }
        }

        foreach (var b in blocksToRemove)
        {
            innerBlocks.Remove(b);
        }

        // fix overlapping blocks
        var overlappingBlocks = innerBlocks.GetOverlappingBlocks(out var block);
        while (block != null)
        {
            foreach (var overlappingBlock in overlappingBlocks)
            {
                block.AddLines(overlappingBlock.Lines.ToList());
                innerBlocks.Remove(overlappingBlock);
            }
            
            overlappingBlocks = innerBlocks.GetOverlappingBlocks(out block);
        }

        return innerBlocks;
    }
    
    // public IPdfTextBlocks FindHorizontalBlocks(IPdfTextBlocks blocks)
    // {
    //     var blockList = new List<IPdfTextBlock>();
    //     blockList.AddRange(blocks);
    //     blocks.Clear();
    //     foreach (var block in blockList)
    //     {
    //         var fixedBlocks = FixVerticalGaps(block);
    //         blocks.AddRange(fixedBlocks);
    //     }
    //
    //     return blocks;
    // }
    //
    // private List<IPdfTextBlock> FixVerticalGaps(IPdfTextBlock block)
    // {
    //     var blocks = new List<IPdfTextBlock>();
    //     // var blocksWithMain = new PdfTextBlock();
    //     // blocksWithMain.AddLines(block.Lines);
    //     // foreach (var line in blocksWithMain.Lines)
    //     // {
    //     //     var smallWords = line.Where(w => w.FontSizeAvg + 0.2 < line.MainFontSizeAvg).ToList();
    //     //     line.RemoveAll(smallWords);
    //     // }
    //
    //     block.VerticalGaps = FindVerticalGaps(block.Lines);
    //     
    //     List<Gap> relevantGaps = new();
    //     var biggestGap = block.VerticalGaps.MaxBy(g => g.Size);
    //     if (biggestGap != null && biggestGap.Size > 2 * block.Lines.WordSpaceAvg)
    //     {
    //         relevantGaps.Add(biggestGap);
    //     }
    //     
    //     // if (block.Lines.WordSpaceAvg != null)
    //     // {
    //     //     relevantGaps = block.VerticalGaps.Where(g => g.Size > 2 * block.Lines.WordSpaceAvg).ToList();
    //     // }
    //     // else
    //     // {
    //     //     relevantGaps = new();
    //     //     foreach (var gap in block.VerticalGaps)
    //     //     {
    //     //         var rel = block.Lines.FontSizeAvg / gap.Size;
    //     //         if (rel < 0.5)
    //     //         {
    //     //             relevantGaps.Add(gap);
    //     //         }
    //     //     }
    //     // }
    //     
    //     if (relevantGaps.Count < 1)
    //     {
    //         // no relevant gaps found, add whole block
    //         blocks.Add(block);
    //         return blocks;
    //     }
    //
    //     // add all the blocks left to the gaps
    //     foreach (var gap in relevantGaps)
    //     {
    //         var leftSideWords = block.Words
    //             .Where(w => w.BoundingBox.Centroid.X < gap.First)
    //             .ToList();
    //         if (leftSideWords.Count > 0)
    //         {
    //             var lines = _lineDetector.DetectLines(leftSideWords);
    //             if (lines.Count > 0)
    //             {
    //                 var tb = new PdfTextBlock
    //                 {
    //                     TopDistance = block.TopDistance,
    //                     Page = block.Page
    //                 };
    //                 tb.AddLines(lines);
    //                 blocks.Add(tb);
    //             }
    //         }
    //     }
    //
    //     // add content right to the last gap
    //     var lastGap = relevantGaps[^1];
    //     var mostRightWords = block.Words
    //         .Where(w => w.BoundingBox.Centroid.X > lastGap.Second)
    //         .ToList();
    //     if (mostRightWords.Count > 0)
    //     {
    //         var mostRightLines = _lineDetector.DetectLines(mostRightWords);
    //         if (mostRightLines.Count > 0)
    //         {
    //             var mostRightTb = new PdfTextBlock
    //             {
    //                 TopDistance = block.TopDistance,
    //                 Page = block.Page
    //             };
    //             mostRightTb.AddLines(mostRightLines);
    //             blocks.Add(mostRightTb);
    //         }
    //     }
    //
    //     return blocks;
    // }
    //
    // public List<Gap> FindVerticalGaps(PdfLines lawLines)
    // {
    //     if (lawLines.Count == 0)
    //     {
    //         return new List<Gap>();
    //     }
    //
    //     // get dictionary with lines and lists of PdfRectangles for the words
    //     var wordList = lawLines
    //         .ToDictionary(
    //             line => (LineOnPage)line, 
    //             line => line
    //                 .Where(w => w.HasText) // consider only words with content
    //                 .Select(w => w.BoundingBox).OrderBy(w => w.Left).ToList()
    //             );
    //
    //     // invert the rectangles and find rectangles on X-axis where _no_ word is found
    //     var wordGapList = wordList.ToDictionary(
    //         kvp => kvp.Key,
    //         kvp => FindFreeSpaces(kvp.Value));
    //     
    //     // get the min and max line spans and fill the free spaces in all lines with gaps
    //     var min = lawLines.Select(l => l.Left).Min();
    //     var max = lawLines.Select(l => l.Right).Max();
    //     foreach (var line in wordGapList.Keys)
    //     {
    //         var y = line.BoundingBox.Centroid.Y;
    //         if (line.Left > min)
    //         {
    //             var x1 = min;
    //             var x2 = line.Left;
    //             wordGapList[line].Insert(0, new PdfRectangle(x1, y, x2, y));
    //         }
    //
    //         if (line.Right < max)
    //         {
    //             var x1 = line.Right;
    //             var x2 = max;
    //             wordGapList[line].Add(new PdfRectangle(x1, y, x2, y));
    //         }
    //     }
    //
    //     // get gaps in all lines by intersecting line by line
    //     var checkLine = wordGapList.First().Value;
    //     for (var i = 1; i < wordGapList.Count; i++)
    //     {
    //         // get gaps of next line
    //         var otherLine = wordGapList.Skip(i).First();
    //         checkLine = checkLine.GetIntersections(otherLine.Value);
    //     }
    //
    //     var gaps = checkLine.Select(rect => new Gap(rect.Left, rect.Right));
    //     return gaps.ToList();
    // }
    //
    // private List<PdfRectangle> FindFreeSpaces(List<PdfRectangle> rects)
    // {
    //     // make two lists, ordered by left and right values of the rectangles
    //     var leftOrderedRects = rects.OrderBy(r => r.Left).ToList();
    //     var rightOrderedRects = leftOrderedRects.OrderByDescending(r => r.Right).ToList();
    //     // select range, where to search for gaps
    //     var min = leftOrderedRects.Select(r => r.Left).Min();
    //     var max = leftOrderedRects.Select(r => r.Right).Max();
    //     // loop, starting by the left range of the leftmost rectangle
    //     var freeSpaces = new List<PdfRectangle>();
    //     var stepRange = 0.1;
    //     for (var x = min; x < max; x += stepRange)
    //     {
    //         var overlapping = leftOrderedRects.Where(r => r.Left <= x && r.Right >= x).ToList();
    //         if (overlapping.Count > 0)
    //         {
    //             // if there are overlapping rectangles, continue searching 
    //             // with the value of the overlapping rectangle that goes most
    //             // right from this position
    //             x = overlapping.MaxBy(r => r.Right).Right;
    //             continue;
    //         }
    //
    //         var prev = rightOrderedRects.FirstOrDefault(r => r.Right <= x);
    //         var next = leftOrderedRects.FirstOrDefault(r => r.Left >= x);
    //         var y = prev.Centroid.Y;
    //         freeSpaces.Add(new PdfRectangle(prev.Right, y, next.Left, y));
    //         x = next.Left;
    //     }
    //
    //     return freeSpaces;
    // }
}
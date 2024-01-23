using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Services;

public class BlockDetector: IBlockDetector
{
    private readonly ILineDetector _lineDetector;
    private readonly IOutput _log;

    public BlockDetector(ILineDetector lineDetector, IOutput log)
    {
        _lineDetector = lineDetector;
        _log = log;
    }

    public IPdfTextBlocks FindBlocks(IPdfPage page, double rangeY, double blockVerticalTolerance)
    {
        // find common groups and create blocks
        var lines = page.Lines;
        var groups = lines.FindCommonGroups(rangeY, d => d.TopDistance.Size);
        var groupsSortedByAverage = groups
            .OrderByDescending(g => g.Select(tpl => tpl.Item1).Average())
            .ToList();
        var innerBlocks = new PdfTextBlocks();
        var allLines = new PdfLines();
        allLines.AddRange(lines.ToList());

        // get lines with big distances, create new blocks
        foreach (var grp in groupsSortedByAverage)
        {
            if (grp.Select(tpl => tpl.Item1).Min() > blockVerticalTolerance)
            {
                foreach (var tpl in grp)
                {
                    var tb = new PdfTextBlock
                    {
                        TopDistance = tpl.Item1
                    };
                    tb.AddLine(tpl.Item2);
                    innerBlocks.Add(tb);
                    allLines.Remove(tpl.Item2);
                }
            }
        }

        // for lines with small distances, append to existing blocks
        foreach (var currentLine in allLines)
        {
            var prevIdx = lines.IndexOf(currentLine) - 1;
            var prevLine = lines[prevIdx];
            var block = innerBlocks.FirstOrDefault(b => b.Any(l => l == prevLine));
            if (block == null)
            {
                throw new InvalidOperationException($"block with prev line not found: {prevLine}");
            }

            block.AddLine(currentLine);
        }
        return innerBlocks;
    }
    
    public IPdfTextBlocks FindHorizontalBlocks(IPdfTextBlocks blocks, double blockHorizontalTolerance)
    {
        var blockList = new List<IPdfTextBlock>();
        blockList.AddRange(blocks);
        blocks.Clear();
        foreach (var block in blockList)
        {
            var fixedBlocks = FixVerticalGaps(block, blockHorizontalTolerance);
            blocks.AddRange(fixedBlocks);
        }

        return blocks;
    }
    
    private List<IPdfTextBlock> FixVerticalGaps(IPdfTextBlock block, double blockHorizontalTolerance)
    {
        var blocks = new List<IPdfTextBlock>();
        var verticalGaps = FindVerticalGaps(block.Lines);
        var relevantGaps = verticalGaps.Where(g => g.Size > blockHorizontalTolerance).ToList();
        if (relevantGaps.Count < 1)
        {
            // no relevant gaps found, add whole block
            blocks.Add(block);
            return blocks;
        }

        // add all the blocks left to the gaps
        foreach (var gap in relevantGaps)
        {
            var words = block.Words
                .Where(w => w.BoundingBox.Centroid.X < gap.First)
                .ToList();
            if (words.Count > 0)
            {
                var lines = _lineDetector.DetectLines(words);
                if (lines.Count > 0)
                {
                    var tb = new PdfTextBlock
                    {
                        TopDistance = block.TopDistance,
                        Page = block.Page
                    };
                    tb.AddLines(lines);
                    blocks.Add(tb);
                }
            }
        }

        // add content right to the last gap
        var lastGap = relevantGaps[^1];
        var mostRightWords = block.Words
            .Where(w => w.BoundingBox.Centroid.X > lastGap.Second)
            .ToList();
        if (mostRightWords.Count > 0)
        {
            var mostRightLines = _lineDetector.DetectLines(mostRightWords);
            if (mostRightLines.Count > 0)
            {
                var mostRightTb = new PdfTextBlock
                {
                    TopDistance = block.TopDistance,
                    Page = block.Page
                };
                mostRightTb.AddLines(mostRightLines);
                blocks.Add(mostRightTb);
            }
        }

        return blocks;
    }

    public List<Gap> FindVerticalGaps(PdfLines lawLines)
    {
        if (lawLines.Count == 0)
        {
            return new List<Gap>();
        }

        // get dictionary with lines and lists of PdfRectangles for the words
        var wordList = lawLines
            .ToDictionary(
                line => (LineOnPage)line, 
                line => line
                    .Where(w => w.HasText) // consider only words with content
                    .Select(w => w.BoundingBox).OrderBy(w => w.Left).ToList()
                );

        // invert the rectangles and find rectangles on X-axis where _no_ word is found
        var wordGapList = wordList.ToDictionary(
            kvp => kvp.Key,
            kvp => FindFreeSpaces(kvp.Value));
        
        // get the min and max line spans and fill the free spaces in all lines with gaps
        var min = lawLines.Select(l => l.Left).Min();
        var max = lawLines.Select(l => l.Right).Max();
        foreach (var line in wordGapList.Keys)
        {
            var y = line.BoundingBox.Centroid.Y;
            if (line.Left > min)
            {
                var x1 = min;
                var x2 = line.Left;
                wordGapList[line].Insert(0, new PdfRectangle(x1, y, x2, y));
            }

            if (line.Right < max)
            {
                var x1 = line.Right;
                var x2 = max;
                wordGapList[line].Add(new PdfRectangle(x1, y, x2, y));
            }
        }

        // get gaps in all lines by intersecting line by line
        var checkLine = wordGapList.First().Value;
        for (var i = 1; i < wordGapList.Count; i++)
        {
            // get gaps of next line
            var otherLine = wordGapList.Skip(i).First();
            checkLine = checkLine.GetIntersections(otherLine.Value);
        }

        var gaps = checkLine.Select(pdf => new Gap(pdf.Left, pdf.Right));
        return gaps.ToList();
    }

    private List<PdfRectangle> FindFreeSpaces(List<PdfRectangle> rects)
    {
        // make two lists, ordered by left and right values of the rectangles
        var leftOrderedRects = rects.OrderBy(r => r.Left).ToList();
        var rightOrderedRects = leftOrderedRects.OrderByDescending(r => r.Right).ToList();
        // select range, where to search for gaps
        var min = leftOrderedRects.Select(r => r.Left).Min();
        var max = leftOrderedRects.Select(r => r.Right).Max();
        // loop, starting by the left range of the leftmost rectangle
        var freeSpaces = new List<PdfRectangle>();
        var stepRange = 0.1;
        for (var x = min; x < max; x += stepRange)
        {
            var overlapping = leftOrderedRects.Where(r => r.Left <= x && r.Right >= x).ToList();
            if (overlapping.Count > 0)
            {
                // if there are overlapping rectangles, continue searching 
                // with the value of the overlapping rectangle that goes most
                // right from this position
                x = overlapping.MaxBy(r => r.Right).Right;
                continue;
            }

            var prev = rightOrderedRects.FirstOrDefault(r => r.Right <= x);
            var next = leftOrderedRects.FirstOrDefault(r => r.Left >= x);
            var y = prev.Centroid.Y;
            freeSpaces.Add(new PdfRectangle(prev.Right, y, next.Left, y));
            x = next.Left;
        }

        return freeSpaces;
    }
}
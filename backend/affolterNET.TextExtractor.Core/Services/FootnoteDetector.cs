using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Services;

public class FootnoteDetector: IFootnoteDetector
{
    private readonly IOutput _log;

    public FootnoteDetector(IOutput log)
    {
        _log = log;
    }
    
    public void DetectBottomFootnotes(IPdfPage page, List<Footnote> fn, double mainFontSize)
    {
        for (var i = page.Blocks.Count; i > 0; i--)
        {
            var block = page.Blocks[i - 1];
            // detect lines starting with number or star, where no words are on the left
            var startingWithNumericOrStar = block.Lines.Where(l =>
                {
                    if (l.FirstWordWithText == null)
                    {
                        return false;
                    }

                    if (!l.FirstWordWithText.Text.IsNumericOrStar())
                    {
                        return false;
                    }

                    if (l.FontSizeAvg >= mainFontSize)
                    {
                        return false;
                    }

                    var wordsOnTheLeft = page.Words.Where(w => w.HasText).Except(new List<IWordOnPage> { l.FirstWordWithText }).All(w =>
                        w.BoundingBox.OverlapsY(l.FirstWordWithText.BoundingBox) &&
                        w.BoundingBox.Centroid.X < l.FirstWordWithText.BoundingBox.Centroid.X
                    );

                    if (wordsOnTheLeft)
                    {
                        return false;
                    }

                    return true;
                }).ToDictionary(l => l, v => new List<LineOnPage>());
            
            // for each line, detect at least one other line where the fontsize is smaller than "mainFontSize"
            foreach (var line in startingWithNumericOrStar.Keys)
            {
                var otherLines = block.Lines.Except(new List<LineOnPage> { line }).Where(l =>
                    l.BoundingBox.OverlapsY(line.BoundingBox) &&
                    l.FontSizeAvg < mainFontSize)
                .OrderBy(l => Math.Abs(line.BaseLineY - l.BaseLineY))
                .ToList();
                if (otherLines.Count == 0)
                {
                    continue;
                }
                startingWithNumericOrStar[line].Add(otherLines.First());
            }

            // get name-value lines
            var footnoteBlockFound = false;
            foreach (var kvp in startingWithNumericOrStar)
            {
                if (kvp.Value.Count < 1)
                {
                    continue;
                }

                var footnote = fn.FirstOrDefault(f => f.Id == kvp.Key.FirstWordWithText!.Text);
                if (footnote == null)
                {
                    continue;
                }

                var startingLineIdx = block.Lines.IndexOf(kvp.Value.First());
                if (startingLineIdx < 0)
                {
                    continue;
                }
                
                LineOnPage? nextLine = null;
                var nextIdx = startingWithNumericOrStar.Keys.ToList().IndexOf(kvp.Key) + 1;
                if (nextIdx < startingWithNumericOrStar.Keys.Count)
                {
                    nextLine = startingWithNumericOrStar.Keys.ToList()[nextIdx];
                }
                for (var li = startingLineIdx; li < block.Lines.Count; li++)
                {
                    var bottomLine = block.Lines[li];
                    if (bottomLine == nextLine)
                    {
                        break;
                    }

                    footnoteBlockFound = true;
                    footnote.BottomContents.AddLine(bottomLine);
                }
            }

            // break if we've found the footnote block
            if (footnoteBlockFound)
            {
                break;
            }
        }
    }

    public List<Footnote> DetectInlineFootnotes(IPdfTextBlock block, double mainFontSize, double minBaseLineDiff)
    {
        var list = new List<Footnote>();
        var words = block.Lines.SelectMany(l => l.ToList())
            .Where(w => w.FontSizeAvg < mainFontSize)
            .Where(w => w.HasText)
            .Where(w => w.Text.IsNumericOrStar())
            .ToList();
        foreach (var word in words)
        {
            var i = 0;
            LineOnPage? containingLine = null;
            var box = word.BoundingBox;
            while (containingLine == null && i < 5)
            {
                containingLine = block.Lines
                    .FirstOrDefault(l => 
                        box.Overlaps(l.BoundingBox, -1) && 
                        Math.Abs(l.BaseLineY - word.BaseLineY) > minBaseLineDiff &&
                        Math.Abs(l.BaseLineY - word.BaseLineY) < l.FontSizeAvg &&
                        l.FontSizeAvg > word.FontSizeAvg);
                box = box.Translate(-1, 0);
                i++;
            }
            
            if (containingLine == null)
            {
                continue;
            }
            // take the words to the left and calc the distance
            var distances = containingLine
                .Where(w => w != word)
                .Where(w => w.HasText)
                .Where(w => w.BoundingBox.Centroid.X < word.BoundingBox.Centroid.X)
                .Select(w => new Tuple<double, IWordOnPage>(w.BoundingBox.TopRight.Distance(word.BoundingBox.BottomLeft), w))
                .ToList();
            
            // find the closest
            var closestToTheLeft = distances.MinBy(tpl => tpl.Item1);

            if (closestToTheLeft == null)
            {
                continue;
            }

            var fn = list.FirstOrDefault(f => f.Id == word.Text);
            if (fn == null) {
                fn = new Footnote(word);
            }
            else
            {
                fn.InlineWords.Add(word);
            }

            if (word.Line == null)
            {
                throw new InvalidOperationException("Word has no line");
            }
            
            list.Add(fn);
        }

        return list;
    }
}
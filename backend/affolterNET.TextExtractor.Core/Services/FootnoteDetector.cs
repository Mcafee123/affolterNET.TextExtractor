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
    
    public List<Footnote> DetectBottomFootnotes(IPdfPage page, List<Footnote> fn, double mainFontSize)
    {
        // get footnote captions from page.Lines
        var linesWithNumbersOrStars = FindBottomFootnoteCaptions(page, mainFontSize)
            .ToDictionary(l => l, v => new List<LineOnPage>());
        
        // add the line right to the caption to the dictionary
        AddFirstBottomContentsLines(linesWithNumbersOrStars, page, mainFontSize);

        // add additional lines per captionLine
        AddAdditionalBottomContentsLines(linesWithNumbersOrStars, page);

        // add the lines to the footnotes
        var footnotesWithoutInlineWord = new List<Footnote>();
        foreach (var captionLine in linesWithNumbersOrStars.Keys)
        {
            var bottomContentLines = linesWithNumbersOrStars[captionLine];
            if (bottomContentLines.Count == 0)
            {
                continue;
            }
            
            var footnote = fn.FirstOrDefault(f => f.Id == captionLine.ToString().Trim());
            if (footnote == null)
            {
                _log.Write(EnumLogLevel.Warning, $"Inline footnote with id {captionLine.ToString().Trim()} not found (page: {page.Nr})");
                footnote = new Footnote(captionLine.ToString().Trim());
                footnotesWithoutInlineWord.Add(footnote);
            }

            footnote.BottomContentsCaption = captionLine;
            footnote.BottomContents.AddLines(bottomContentLines);
        }

        return footnotesWithoutInlineWord;
    }

    private void AddAdditionalBottomContentsLines(Dictionary<LineOnPage, List<LineOnPage>> linesWithNumbersOrStars, IPdfPage page)
    {
        foreach (var captionLine in linesWithNumbersOrStars.Keys)
        {
            var firstTextLine = linesWithNumbersOrStars[captionLine].FirstOrDefault();
            if (firstTextLine == null)
            {
                continue;
            }
            var firstTextLineBlock = page.Blocks.FirstOrDefault(b => b.Lines.Contains(firstTextLine));
            if (firstTextLineBlock == null)
            {
                throw new InvalidOperationException("First text line block not found");
            }
            var startingLineIdx = firstTextLineBlock.Lines.IndexOf(firstTextLine);
            if (startingLineIdx < 0)
            {
                throw new InvalidOperationException("Line not found in block");
            }
            for (var li = startingLineIdx; li < firstTextLineBlock.Lines.Count; li++)
            {
                var bottomLine = firstTextLineBlock.Lines[li];
                if (bottomLine == firstTextLine)
                {
                    continue;
                }
                
                // if line is the caption of the next, continue
                if (linesWithNumbersOrStars.Keys.Any(l => l == bottomLine))
                {
                    continue;
                }
                
                // if line is the first line of the next, break
                if (linesWithNumbersOrStars.Values.Any(l => l.Contains(bottomLine)))
                {
                    break;
                }
            
                linesWithNumbersOrStars[captionLine].Add(bottomLine);
            }
        }
    }

    private void AddFirstBottomContentsLines(Dictionary<LineOnPage, List<LineOnPage>> linesWithNumbersOrStars, IPdfPage page, double mainFontSize)
    {
        foreach (var captionLine in linesWithNumbersOrStars.Keys)
        {
            var linesToTheRight = page.Lines.Except(linesWithNumbersOrStars.Keys)
                .Where(l =>
                    captionLine.BoundingBox.OverlapsY(l.BoundingBox) &&
                    captionLine.BoundingBox.Centroid.X < l.BoundingBox.Left &&
                    l.FontSizeAvg < mainFontSize
                )
                .OrderBy(l => Math.Abs(captionLine.BaseLineY - l.BaseLineY))
                .ToList();

            if (linesToTheRight.Count == 0)
            {
                continue;
            }

            var sameLineAsCaption = linesToTheRight.First();
            linesWithNumbersOrStars[captionLine].Add(sameLineAsCaption);
        }
    }

    private List<LineOnPage> FindBottomFootnoteCaptions(IPdfPage page, double mainFontSize)
    {
        var captionLines = page.Blocks.SelectMany(b => b.Lines)
            .Where(l => l.FontSizeAvg < mainFontSize)
            .Where(l => l.FirstWordWithText is { HasText: true } && l.ToString().Trim() == l.FirstWordWithText.Text)
            .Where(l => l.FirstWordWithText!.Text.IsNumericOrStar())
            .ToList();
        return captionLines;
    }
}
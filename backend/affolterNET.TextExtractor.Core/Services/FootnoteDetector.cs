using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Services;

public class FootnoteDetector : IFootnoteDetector
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
                        l.BaseLineY < word.BaseLineY &&
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
                .Select(w =>
                    new Tuple<double, IWordOnPage>(w.BoundingBox.TopRight.Distance(word.BoundingBox.BottomLeft), w))
                .ToList();

            // find the closest
            var closestToTheLeft = distances.MinBy(tpl => tpl.Item1);

            if (closestToTheLeft == null)
            {
                continue;
            }

            var fn = list.FirstOrDefault(f => f.Id == word.Text);
            if (fn == null)
            {
                fn = new Footnote(block.Page, word, closestToTheLeft.Item2);
            }
            else
            {
                fn.InlineWords.Add(word);
            }

            if (word.Line == null)
            {
                throw new InvalidOperationException("word has no line");
            }

            list.Add(fn);
        }

        return list;
    }

    public List<Footnote> DetectBottomFootnotes(IPdfPage page, List<Footnote> fn, double mainFontSize,
        DetectFootnotesStep.DetectFootnotesStepSettings settings)
    {
        // get footnote captions from page.Lines
        var linesWithNumbersOrStars = FindBottomFootnoteCaptions(page, fn, mainFontSize, settings)
            .ToDictionary(l => l, v => new List<LineOnPage>());

        // add the line right to the caption to the dictionary
        AddFirstBottomContentsLines(linesWithNumbersOrStars, page);

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
                _log.Write(EnumLogLevel.Warning,
                    $"Inline footnote with id {captionLine.ToString().Trim()} not found (page: {page.Nr})");
                footnote = new Footnote(captionLine.ToString().Trim(), page);
                footnotesWithoutInlineWord.Add(footnote);
            }

            footnote.BottomContentsCaption = captionLine;
            footnote.BottomContents.AddLines(bottomContentLines);
        }

        return footnotesWithoutInlineWord;
    }

    private void AddAdditionalBottomContentsLines(Dictionary<LineOnPage, List<LineOnPage>> linesWithNumbersOrStars,
        IPdfPage page)
    {
        foreach (var captionLine in linesWithNumbersOrStars.Keys)
        {
            var firstTextLine = linesWithNumbersOrStars[captionLine].FirstOrDefault();
            if (firstTextLine == null)
            {
                continue;
            }

            var firstTextLineBlock = page.Blocks.TextBlocks.FirstOrDefault(b => b.Lines.Contains(firstTextLine));
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

    private void AddFirstBottomContentsLines(Dictionary<LineOnPage, List<LineOnPage>> linesWithNumbersOrStars,
        IPdfPage page)
    {
        var lineList = linesWithNumbersOrStars.Keys.ToList();
        for (var i = lineList.Count - 1; i > -1; i--)
        {
            var captionLine = lineList[i];
            var linesToTheRight = page.Lines.Except(linesWithNumbersOrStars.Keys)
                .Where(l =>
                    captionLine.BoundingBox.OverlapsY(l.BoundingBox) &&
                    captionLine.BoundingBox.Centroid.X < l.BoundingBox.Left
                )
                .OrderBy(l => Math.Abs(captionLine.BaseLineY - l.BaseLineY))
                .Select(l => new { Line = l, DistanceX = l.BoundingBox.Left - captionLine.BoundingBox.Right })
                .ToList();

            // var maxDistance = captionLine.FirstWordWithText!.Letters.First().Width * 10;
            // linesToTheRight.RemoveAll(ld => ld.DistanceX > maxDistance);

            if (linesToTheRight.Count == 0)
            {
                continue;
            }

            var sameLineAsCaption = linesToTheRight.First();
            linesWithNumbersOrStars[captionLine].Add(sameLineAsCaption.Line);
        }
    }

    private List<LineOnPage> FindBottomFootnoteCaptions(IPdfPage page, List<Footnote> fn, double mainFontSize,
        DetectFootnotesStep.DetectFootnotesStepSettings settings)
    {
        var inlineWords = fn.SelectMany(f => f.InlineWords);
        var captionLines = page.Blocks.TextBlocks.SelectMany(b => b.Lines)
            .Where(l => l.All(w => !inlineWords.Contains(w)))
            .Where(l => l.FontSizeAvg < mainFontSize)
            .Where(l => l.FirstWordWithText is { HasText: true } && l.ToString().Trim() == l.FirstWordWithText.Text)
            .Where(l => l.FirstWordWithText!.Text.IsNumericOrStar())
            .ToList();

        // if multiple captions with the same footnote id are found, select the ones lower on the page
        var multipleIds = captionLines.GroupBy(l => l.FirstWordWithText!.Text)
            .Where(g => g.Count() > 1)
            .ToList();
        foreach (var grp in multipleIds)
        {
            var lowestOnPage = grp.MinBy(l => l.BaseLineY);
            foreach (var upperLine in grp.Except([lowestOnPage]))
            {
                _log.Write(EnumLogLevel.Trace,
                    $"Removing caption line {upperLine} because there is a line for {upperLine.FirstWordWithText!.Text} lower on the page");
                captionLines.Remove(upperLine);
            }
        }

        // remove already added footnotes from found captions
        foreach (var footnote in fn)
        {
            var captionLine = captionLines.FirstOrDefault(l => l.FirstWordWithText!.Text == footnote.Id);
            if (captionLine != null && footnote.BottomContents.Lines.Count > 0)
            {
                captionLines.Remove(captionLine);
            }
        }

        // remove outliers
        if (captionLines.Count > 0)
        {
            // get left text border, where most of the blocks start
            var leftTextStart = page.Blocks.FindCommonGroups(settings.LeftBorderGroupRange, b => b.BoundingBox.Left);
            var first = leftTextStart.FirstOrDefault();
            if (first != null)
            {
                // get left borders of all captionlines
                var distances = captionLines
                    .FindCommonGroups(settings.LeftBorderGroupRange, line => Math.Abs(line.BoundingBox.Left - first.AvgValue)).ToList();
                foreach (var grp in distances)
                {
                    if (grp.AvgValue > settings.MaxDistFromLeft)
                    {
                        foreach (var l in grp)
                        {
                            _log.Write(EnumLogLevel.Trace,
                                $"Removing caption line {l.Obj} with left border outlier (distance: {l.Value} > {settings.MaxDistFromLeft})");
                            captionLines.Remove(l.Obj);
                        }
                    }
                }
            }
        }
        
        return captionLines;
    }
}
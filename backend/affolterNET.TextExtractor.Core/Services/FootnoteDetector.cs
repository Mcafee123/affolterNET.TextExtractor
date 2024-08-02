using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services.Interfaces;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.PdfFonts;

namespace affolterNET.TextExtractor.Core.Services;

public class FootnoteDetector : IFootnoteDetector
{
    private readonly IOutput _log;

    public FootnoteDetector(IOutput log)
    {
        _log = log;
    }

    public List<Footnote> DetectFootnotes(IPdfDoc document, double minBaselineDiff)
    {
        foreach (var page in document.Pages)
        {
            foreach (var block in page.Blocks.TextBlocks)
            {
                try
                {
                    var mainFontSize = (double)9;
                    var mainFontSetting = block.FontSizes?.GetMainFontSetting();
                    if (mainFontSetting != null)
                    {
                        mainFontSize = mainFontSetting.AvgFontSize;
                    }

                    // detect the inline footnotes
                    var footnotes = DetectInlineFootnotes(block, mainFontSize, minBaselineDiff);
                    document.Footnotes.AddRange(footnotes);
                }
                catch (Exception ex)
                {
                    _log.Write(EnumLogLevel.Error, $"Error detecting footnotes on page {page.Nr}: {ex.Message}");
                }
            }
        }

        for (var i = document.Pages.Count - 1; i > -1; i--)
        {
            var page = document.Pages[i];
            // detect the explanation in the page footer
            DetectBottomFootnotes(page, document);
        }

        return document.Footnotes;
    }

    public List<Footnote> DetectInlineFootnotes(IPdfTextBlock block, double mainFontSize, double minBaseLineDiff)
    {
        var list = new List<Footnote>();
        var allLines = block.Lines.ToList();
        foreach (var line in block.Lines)
        {
            // detect overlapping lines
            // check with a pdf rectangle a bit more to the left to overlap even line ends
            var bottomLeft = new PdfPoint(line.BoundingBox.Left - 1, line.BoundingBox.Bottom);
            var shiftedRect = new PdfRectangle(bottomLeft, line.BoundingBox.TopRight);
            var overlapping = allLines.Where(l =>
                    l != line && l.BoundingBox.Overlaps(shiftedRect))
                .ToList();
            if (overlapping.Count > 1)
            {
                throw new InvalidOperationException("too many lines are overlapping");
            }

            if (overlapping.Count > 0)
            {
                overlapping.Add(line);
                var upper = overlapping.MaxBy(l => l.BaseLineY)!;
                var lower = overlapping.MinBy(l => l.BaseLineY)!;
                allLines.Remove(upper);
                allLines.Remove(lower);
                var potentialFootnotes = upper
                    .Where(w => w.FontSizeAvg < lower.FontSizeAvg)
                    .Where(w => w.HasText)
                    .Where(w => w.Text.IsNumericOrStar())
                    .ToList();

                foreach (var potFn in potentialFootnotes)
                {
                    // take the words to the left in the lower line and calc the distance
                    var distances = lower
                        .Where(w => w.HasText)
                        .Where(w => w.BoundingBox.Centroid.X < potFn.BoundingBox.Centroid.X) // words to the left
                        .Where(w => w.BoundingBox.Top >
                                    potFn.BoundingBox.Centroid.Y -
                                    potFn.BoundingBox.Height /
                                    4) // overlapping y to one quarter of the height of the footnote
                        .Select(w =>
                            new Tuple<double, IWordOnPage>(
                                w.BoundingBox.TopRight.Distance(potFn.BoundingBox.BottomLeft), w))
                        .ToList();

                    // find the closest
                    var closestToTheLeft = distances.MinBy(tpl => tpl.Item1);

                    if (closestToTheLeft == null)
                    {
                        continue;
                    }

                    var fn = list.FirstOrDefault(f => f.Id == potFn.Text);
                    if (fn == null)
                    {
                        fn = new Footnote(block.Page, potFn, closestToTheLeft.Item2);
                    }
                    else
                    {
                        fn.InlineWords.Add(potFn);
                    }

                    list.Add(fn);
                }
            }
        }

        return list;
    }

    public void DetectBottomFootnotes(IPdfPage page, IPdfDoc document)
    {
        var allLines = page.Blocks.TextBlocks.SelectMany(b => b.Lines).ToList();
        // all lines with captions or stars
        var potentialFootnoteCaptions = allLines
            .Where(l => l.FirstWordWithText?.Text.IsNumericOrStar() ?? false)
            .ToDictionary(l => l, _ => new List<LineOnPage>());
        var documentMainFontSize = document.FontSizes?.GetMainFontSetting()?.AvgFontSize ?? 9;
        // add lines to the right that overlap
        foreach (var line in potentialFootnoteCaptions.Keys)
        {
            // find lines to the right
            var linesWithContent = allLines
                .Where(l => l != line &&
                            l.BoundingBox.OverlapsY(line.BoundingBox) &&
                            l.BoundingBox.Left > line.BoundingBox.Right &&
                            l.FontSizeAvg < documentMainFontSize)
                .ToList();
            if (linesWithContent.Count > 1)
            {
                var closestByBaseline = linesWithContent.MinBy(l => Math.Abs(l.BaseLineY - line.BaseLineY));
                linesWithContent.Clear();
                linesWithContent.Add(closestByBaseline!);
            }

            var dictEntry = potentialFootnoteCaptions[line];
            dictEntry.AddRange(linesWithContent);
        }

        // reverse dictionary to start from the bottom
        var addedCaptions = new List<string>();
        var reversedDict = potentialFootnoteCaptions.OrderBy(kvp => kvp.Key.BoundingBox.Bottom);
        foreach (var potFn in reversedDict)
        {
            if (potFn.Value.Count == 0)
            {
                continue;
            }

            if (potFn.Key.Count(w => w.HasText) > 1)
            {
                _log.Write(EnumLogLevel.Warning,
                    $"Multiple words in caption line {potFn.Key} (page: {page.Nr})");
            }

            var bottomFootnoteCaption = potFn.Key.FirstWordWithText!;
            var fn = document.Footnotes.FirstOrDefault(f => f.Id == bottomFootnoteCaption.Text);
            if (fn == null)
            {
                continue;
            }

            if (fn.BottomContents.Lines.Count > 0 && !addedCaptions.Contains(bottomFootnoteCaption.Text))
            {
                _log.Write(EnumLogLevel.Warning,
                    $"Bottom footnote with id {fn.Id} already has content (page: {page.Nr})");
            }

            fn.BottomContentsCaption = potFn.Key;
            fn.BottomContents.AddLines(potFn.Value.ToArray());
            addedCaptions.Add(bottomFootnoteCaption.Text);
            AddAdditionalBottomContentLines(fn, page);
        }
    }

    private void AddAdditionalBottomContentLines(Footnote fn, IPdfPage page)
    {
        var firstTextLine = fn.BottomContents.Lines.FirstOrDefault();
        if (firstTextLine == null)
        {
            return;
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
            if (page.Document.Footnotes.Any(f => f != fn && f.BottomContentsCaption == bottomLine))
            {
                continue;
            }

            // if line is the first line of the next, break
            if (page.Document.Footnotes.Any(f => f != fn && f.BottomContents.Lines.Contains(bottomLine)))
            {
                break;
            }

            // only add lines if they match the left border
            if (Math.Abs(bottomLine.BoundingBox.Left - firstTextLine.BoundingBox.Left) < 1)
            {
                fn.BottomContents.AddLines(bottomLine);
            }
        }
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
}
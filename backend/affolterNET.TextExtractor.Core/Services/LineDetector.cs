using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Services.Interfaces;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Services;

public class LineDetector: ILineDetector
{
    private readonly IOutput _log;

    public LineDetector(IOutput log)
    {
        _log = log;
    }
    
    public PdfLines DetectLines(List<IWordOnPage> words, double baseLineMatchingRange)
    {
        // sometimes, the order of words is not according to the read-direction.
        // words more on the right can appear in the list before the first word on
        // the same line, because they have a slightly bigger y. detect lines and bring words in order
        var lines = new PdfLines();
        if (!words.Any())
        {
            return lines;
        }

        var pageNrs = words.Select(w => w.PageNr).Distinct().ToList();
        if (pageNrs.Count > 1)
        {
            throw new InvalidOperationException("Multiple pages in one list of words");
        }

        var pageNr = pageNrs.First();

        // horizontal
        var wordsHorizontal = words.Where(w => w.TextOrientation == TextOrientation.Horizontal).ToList();
        var horizontalLines = ReadHorizontalWords(wordsHorizontal, baseLineMatchingRange);
        lines.AddRange(horizontalLines.ToList());
        
        // rotate270
        var wordsRotate270 = words
            .Where(w => w.TextOrientation == TextOrientation.Rotate270)
            .OrderBy(w => w.BoundingBox.Left).ThenBy(w => w.BoundingBox.Bottom)
            .ToList();
        foreach (var word in wordsRotate270)
        {
            if (string.IsNullOrWhiteSpace(word.Text) || lines.ContainsWord(word))
            {
                // word was already added
                continue;
            }

            var line = ReadLineUp(wordsRotate270, wordsRotate270.IndexOf(word));
            lines.Add(line);
        }
        
        // rotate90
        var wordsRotate90 = words.Where(w => w.TextOrientation == TextOrientation.Rotate90).ToList();
        if (wordsRotate90.Any())
        {
            throw new NotImplementedException($"words with orientation {TextOrientation.Rotate90} found on page {pageNr}");
        }

        // rotate180
        var wordsRotate180 = words.Where(w => w.TextOrientation == TextOrientation.Rotate180).ToList();
        if (wordsRotate180.Any())
        {
            throw new NotImplementedException($"words with orientation {TextOrientation.Rotate180} found found on page {pageNr}");
        }
        
        // rotateOther
        var wordsRotateOther = words.Where(w => w.TextOrientation == TextOrientation.Other).ToList();
        if (wordsRotateOther.Any())
        {
            throw new NotImplementedException($"words with orientation {TextOrientation.Other} found found on page {pageNr}");
        }

        return lines;
    }

    private List<LineOnPage> ReadHorizontalWords(List<IWordOnPage> wordsHorizontal, double baseLineMatchingRange)
    {
        var lines = new List<LineOnPage>();
        var clone = new List<IWordOnPage>();
        clone.AddRange(wordsHorizontal);
        while (clone.Count > 0)
        {
            var line = ReadLineHorizontal(clone, 0, baseLineMatchingRange);
            if (line.Any(w => !string.IsNullOrWhiteSpace(w.Text)))
            {
                lines.Add(line);
            }
        }

        return lines;
    }

    private LineOnPage ReadLineHorizontal(List<IWordOnPage> words, int startIndex = 0, double range = 0.2)
    {
        var line = new LineOnPage(new List<IWordOnPage>(), -1);
        if (words.Count <= startIndex)
        {
            return line;
        }
        var initialWord = words[startIndex];
        line.PageNr = initialWord.PageNr;
        var overlappingWords = words.Where(w => Math.Abs(w.BaseLineY - initialWord.BaseLineY) <= range && Math.Abs(w.FontSizeAvg - initialWord.FontSizeAvg) <= range).ToList();
        foreach (var lineMember in overlappingWords)
        {
            line.Add(lineMember);
            words.Remove(lineMember);
        }

        return line;
    }
    
    private LineOnPage ReadLineUp(List<IWordOnPage> words, int startIndex = 0)
    {
        var line = new LineOnPage(new List<IWordOnPage>(), -1);
        if (words.Count <= startIndex)
        {
            return line;
        }
        var initialWord = words[startIndex];
        line.PageNr = initialWord.PageNr;
        var boundingBox = initialWord.BoundingBox;
        foreach (var w in words.Where(w => w.PageNr == initialWord.PageNr))
        {
            var box = w.BoundingBox;
            if (box.OverlapsX(boundingBox))
            {
                line.Add(w);
                // increase BoundingBox (if height is larger) to reach all the contents of a line
                boundingBox = line.BoundingBox.Height > boundingBox.Height ? line.BoundingBox : boundingBox;
            }
        }

        return line;
    }
}
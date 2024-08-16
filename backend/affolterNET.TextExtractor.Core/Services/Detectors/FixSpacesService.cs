using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Services.Interfaces;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Services.Detectors;

public class FixSpacesService : IFixSpacesService
{
    private readonly ILineDetector _lineDetector;
    private readonly IOutput _log;
    
    public FixSpacesService(ILineDetector lineDetector, IOutput log)
    {
        _lineDetector = lineDetector;
        _log = log;
    }
    
    public int FixSpaces(IPdfDoc doc, double baseLineMatchingRange)
    {
        var missingSpacesCount = 0;
        var allBlocks = doc.Pages.SelectMany(p => p.Blocks.TextBlocks).ToList();
        foreach (var block in allBlocks)
        {
            var missingSpaces = new List<IWordOnPage>();
            foreach (var line in block.Lines)
            {
                if (line.Count < 2)
                {
                    continue;
                }

                var spaces = CreateMissingSpaces(line);
                missingSpaces.AddRange(spaces);
            }

            if (missingSpaces.Count == 0)
            {
                continue;
            }

            // check if there is not a footnote
            missingSpaces = missingSpaces.Where(s =>
                !doc.Footnotes.SelectMany(fn => fn.InlineWords)
                    .Where(w => w.PageNr == block.PageNr)
                    .Any(inlineWord => inlineWord.BoundingBox.Overlaps(s.BoundingBox))).ToList();

            if (missingSpaces.Count == 0)
            {
                continue;
            }

            // add spaces to block words
            block.AddWords(missingSpaces.ToArray());
            block.DetectLines(_lineDetector, baseLineMatchingRange);
            missingSpacesCount += missingSpaces.Count;
        }

        return missingSpacesCount;
    }

    private List<IWordOnPage> CreateMissingSpaces(LineOnPage line)
    {
        var spacesToAdd = new List<IWordOnPage>();
        for (var i = 0; i < line.Count - 1; i++)
        {
            var currentWord = line[i];
            var nextWord = line[i + 1];
            if (!currentWord.HasText)
            {
                continue;
            }

            var gapWidth = Math.Abs(nextWord.BoundingBox.Left - currentWord.BoundingBox.Right);
            if (nextWord.HasText && gapWidth > line.WordSpaceAvg / 2 && gapWidth < line.WordSpaceAvg * 1.5)
            {
                // missing space found
                var space = CreateSpaceWord(currentWord, line, nextWord);
                spacesToAdd.Add(space);
            }
        }

        return spacesToAdd;
    }

    private IWordOnPage CreateSpaceWord(IWordOnPage currentWord, ILineOnPage line, IWordOnPage nextWord)
    {
        var newWord = currentWord.Clone();
        var bottomLeft = new PdfPoint(currentWord.BoundingBox.Right, currentWord.BaseLineY);
        var bottomRight = new PdfPoint(nextWord.BoundingBox.Left, currentWord.BaseLineY);
        var topRight = new PdfPoint(nextWord.BoundingBox.Left, line.BoundingBox.Top);
        var newGlyphRectangle = new PdfRectangle(bottomLeft, topRight);
        var firstLetter = newWord.Letters.First();
        var l = new Letter(" ", newGlyphRectangle, bottomLeft, bottomRight,
            topRight.X - bottomLeft.X, firstLetter.FontSize, firstLetter.Font, firstLetter.RenderingMode,
            firstLetter.StrokeColor, firstLetter.FillColor, firstLetter.PointSize, firstLetter.TextSequence);
        newWord.ChangeWord([l]);
        return newWord;
    }
}
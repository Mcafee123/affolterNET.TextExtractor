using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class DetectFootnotesStep: IPipelineStep
{
    private readonly IOutput _log;

    public DetectFootnotesStep(IOutput log)
    {
        _log = log;
    }
    
    public class DetectFootnotesStepSettings: IStepSettings
    {
    
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Detecting footnotes");
        var fontSizes = context.Document!.FontSizes;
        var mainFontSize = (double)10;
        var mainFontSetting = fontSizes.ToList().MaxBy(fs => fs.WordCount);
        if (mainFontSetting != null)
        {
            mainFontSize = mainFontSetting.AvgFontSize;
        }

        foreach (var page in context.Document.Pages)
        {
            foreach (var block in page.Blocks)
            {
                var footnotes = DetectInlineFootnotes(block, mainFontSize);
            }

            // var footnotes = page.Lines.DetectFootnotes();
            // page.Footnotes.AddRange(footnotes);
        }
    }

    private List<Footnote> DetectInlineFootnotes(IPdfTextBlock block, double mainFontSize)
    {
        var words = block.Lines.SelectMany(l => l.ToList()).Select(w => new { Word = w, Letters = w.Letters.Where(l => l.Value.IsNumericOrStar()) }).ToList();
        foreach (var w in words)
        {
            
        }

        var list = new List<Footnote>();
        foreach (var line in block.Lines)
        {
            foreach (var word in line.Where(w => w.HasText && w.Text.IsNumericOrStar() && w.FontSizeAvg < mainFontSize))
            {
                // is it a fraction?
                if (word.IsFraction(line))
                {
                    // abc1/3
                    // ignore
                    continue;
                }
                
                // is it the first word in a line?
                if (line.IndexOf(word) == 0)
                {
                    // footnotes have a word to the left where they belong to
                    // the most left word is never a footnote
                    continue;
                }
            }
        }

        return list;
    }

    // private void DetectInlineFootnotes1(IPdfTextBlock block)
    // {
    //     foreach (var line in block.Lines)
    //     {
    //         List<IWordOnPage> foundInlineWords = new();
    //         foreach (var word in line)
    //         {
    //             // content?
    //             if (!word.HasText)
    //             {
    //                 continue;
    //             }
    //
    //             // check if text is small enough
    //             if (word.BoundingBox.Height >= footNoteMaxSize)
    //             {
    //                 continue;
    //             }
    //
    //             // check if it's numeric
    //             if (!word.Text.IsNumericOrStar())
    //             {
    //                 continue;
    //             }
    //
    //             // is it a fraction?
    //             if (word.IsFraction(line))
    //             {
    //                 // abc1/3
    //                 // ignore
    //                 continue;
    //             }
    //
    //             // is it the first word in a line?
    //             if (line.IndexOf(word) == 0)
    //             {
    //                 // footnotes have a word to the left where they belong to
    //                 // the most left word is never a footnote
    //                 continue;
    //             }
    //
    //             // take the words to the left and calc the distance
    //             var distanceDict = line
    //                 .Where(w => w.HasText)
    //                 .Where(w => line.IndexOf(w) < line.IndexOf(word))
    //                 .Where(w => word.BaseLineY > line.BaseLineY)
    //                 .ToImmutableSortedDictionary(
    //                     w => w.BoundingBox.TopRight.Distance(word.BoundingBox.BottomLeft),
    //                     w => w);
    //
    //             // find the closest
    //             var closestToTheLeft = distanceDict.Keys.Any()
    //                 ? distanceDict[distanceDict.Keys.Min()]
    //                 : null;
    //             if (closestToTheLeft != null)
    //             {
    //                 var fn = pdfDoc.Footnotes.FirstOrDefault(f => f.Number == int.Parse(word.Text));
    //                 if (fn == null)
    //                 {
    //                     continue;
    //                 }
    //
    //                 if ((closestToTheLeft.Text == "m" || closestToTheLeft.Text == "(m") && word.Text == "3")
    //                 {
    //                     // m3: Kubikmeter, found in
    //                     // - /Bundesgesetz/747.11/2023-07-01/fedlex-data-admin-ch-eli-cc-40-62_63_63-20230701-de-pdf-a.pdf
    //                     // - Departementsverordnung/641.811.31/2001-01-01/fedlex-data-admin-ch-eli-cc-2000-457-20010101-de-pdf-a.pdf
    //                     continue;
    //                 }
    //
    //                 var foundInline = pdfDoc.Footnotes.Where(f => f.InlineWords.Count > 0).ToList();
    //                 var biggestFound = foundInline.Count > 0 ? foundInline.Max(f => f.Number) : 0;
    //                 if (Math.Abs(fn.Number - biggestFound) > 15)
    //                 {
    //                     // ignore inline footnote because it's too far away from the others
    //                     continue;
    //                 }
    //
    //                 fn.InlineWords.Add(word);
    //                 foundInlineWords.Add(word);
    //                 closestToTheLeft.Footnote = fn;
    //             }
    //         }
    //
    //         // remove inline words
    //         foreach (var word in foundInlineWords)
    //         {
    //             line.Remove(word);
    //         }
    //     }
    // }
}
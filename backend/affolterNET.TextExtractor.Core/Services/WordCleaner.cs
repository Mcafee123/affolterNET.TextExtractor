using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services.Interfaces;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Services;

public class WordCleaner: IWordCleaner
{
    private readonly IOutput _log;

    public WordCleaner(IOutput log)
    {
        _log = log;
    }
    
    public void SeparatePunctuation(IPipelineContext context, char[] startPunctuationChars, char[] endPunctuationChars)
    {
        // start punctuation
        foreach (var page in context.Document!.Pages)
        {
            var words = page.Words
                .Where(w => w.HasText)
                .Select(w =>
                {
                    var first = w.Letters.First();
                    var others = w.Letters.Where(l => l != first).ToList();
                    return new { First = first, Others = others, Word = w };
                })
                .Where(fo => startPunctuationChars.Contains(fo.First.Value.First()) && fo.Others.Count > 0)
                .ToList();
            foreach (var fo in words)
            {
                SeparatePunctuationWord(fo.Word, page, fo.First, fo.Others);
            }
        }
        
        // end punctuation
        foreach (var page in context.Document!.Pages)
        {
            var words = page.Words
                .Where(w => w.HasText)
                .Select(w =>
                {
                    var last = w.Letters.Last();
                    var others = w.Letters.Where(l => l != last).ToList();
                    return new { Last = last, Others = others, Word = w };
                })
                .Where(fo => endPunctuationChars.Contains(fo.Last.Value.First()) && fo.Others.Count > 0)
                .ToList();
            foreach (var fo in words)
            {
                SeparatePunctuationWord(fo.Word, page, fo.Last, fo.Others);
            }
        }
    }

    private void SeparatePunctuationWord(IWordOnPage word, IPdfPage page, Letter singleLetter, List<Letter> others)
    {
        page.RemoveWord(word);
        var punct = word.Clone();
        punct.ChangeWord([singleLetter]);
        page.AddWord(punct);
        
        var other = word.Clone();
        other.ChangeWord(others);
        page.AddWord(other);
    }

    public void FixMixedBaselineWords(IPipelineContext context, double minBaseLineDiff)
    {
        foreach (var page in context.Document!.Pages)
        {
            var words = page.Words
                .Where(w => w.HasText)
                .Where(w => w.BaseLineGroups.MinMaxDiff > minBaseLineDiff)
                .ToList();
            foreach (var word in words)
            {
                FixMixedBaselineWord(word, page);
            }
        }
    }

    private void FixMixedBaselineWord(IWordOnPage word, IPdfPage page)
    {
        page.RemoveWord(word);
        var grpId = -1;
        var letterGroups = new List<List<Letter>>();
        foreach (var letter in word.Letters)
        {
            var newGroup = word.BaseLineGroups.GetGroup(letter.StartBaseLine.Y);
            if (newGroup.GroupId != grpId)
            {
                letterGroups.Add(new List<Letter>());
            }

            grpId = newGroup.GroupId;
            letterGroups.Last().Add(letter);
        }
        
        foreach (var group in letterGroups)
        {
            var newWord = word.Clone();
            newWord.ChangeWord(group);
            page.AddWord(newWord);
        }
    }

    public void FixMixedLetterTypeWords(IPipelineContext context)
    {
        foreach (var page in context.Document!.Pages)
        {
            var words = page.Words
                .Where(w => w.HasText)
                .Where(w =>
                {
                    bool hasNumberOrStar = w.Letters.Any(l => l.Value.IsNumericOrStar());
                    bool hasOther =
                        w.Letters.Any(l => !string.IsNullOrWhiteSpace(l.Value) && !l.Value.IsNumericOrStar());
                    return hasNumberOrStar && hasOther;
                })
                .ToList();
            foreach (var word in words)
            {
                FixMixedLetterTypeWord(word, page);
            }
        }
    }

    private void FixMixedLetterTypeWord(IWordOnPage word, IPdfPage page)
    {
        page.RemoveWord(word);
        var firstPart = new List<Letter>();
        foreach (var letter in word.Letters)
        {
            if (letter.Value.IsNumericOrStar())
            {
                break;
            }
            firstPart.Add(letter);
        }

        if (firstPart.Count > 0)
        {
            var firstWord = word.Clone();
            firstWord.ChangeWord(firstPart);
            page.AddWord(firstWord);
        }

        var numbersPart = new List<Letter>();
        foreach (var letter in word.Letters.Skip(firstPart.Count))
        {
            if (!letter.Value.IsNumericOrStar())
            {
                break;
            }
            numbersPart.Add(letter);
        }

        var numbersWord = word.Clone();
        numbersWord.ChangeWord(numbersPart);
        page.AddWord(numbersWord);
            
        var lastPart = new List<Letter>();
        foreach (var letter in word.Letters.Skip(firstPart.Count + numbersPart.Count))
        {
            if (letter.Value.IsNumericOrStar())
            {
                break;
            }
            lastPart.Add(letter);
        }

        if (lastPart.Count > 0)
        {
            var lastWord = word.Clone();
            lastWord.ChangeWord(lastPart);
            page.AddWord(lastWord);
        }
    }

    public void RemoveBigSpaces(IPipelineContext context, CleanWordsStep.CleanWordsStepSettings settings)
    {
        var removedTotal = 0;
        foreach (var page in context.Document!.Pages)
        {
            var wordsToRemove = page.Words.Where(w =>
                string.IsNullOrWhiteSpace(w.Text) && w.Letters.Count == 1 &&
                w.Letters.Any(l => l.PointSize > settings.BigSpacesSize))
                .ToList();
            foreach (var word in wordsToRemove)
            {
                page.RemoveWord(word);
            }
            removedTotal += wordsToRemove.Count;
        }
        
        if (removedTotal > 0)
        {
            _log.Write(EnumLogLevel.Debug, $"Big spaces removed: {removedTotal}");
        }
    }
}
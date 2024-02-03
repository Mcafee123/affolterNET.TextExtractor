using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class CleanWordsStep: IPipelineStep
{
    private readonly IOutput _log;

    public CleanWordsStep(IOutput log)
    {
        _log = log;
    }
    
    public class CleanWordsStepSettings: IStepSettings
    {
        public double BigSpacesSize { get; set; } = 100;
        public double MinBaseLineDiff { get; set; } = 1.5;
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Cleaning words");
        var settings = context.GetSettings<CleanWordsStepSettings>();
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadWordsStep)} before this step");
        }
        
        // remove useless big spaces (Fedlex-Laws)
        RemoveBigSpaces(context, settings);
        
        // separate words with numbers and other signs
        FixMixedBaselineWords(context, settings.MinBaseLineDiff);
        
        // separate words with numbers and other signs
        // not set at the moment bc. of stuff like "m3".
        // FixMixedLetterTypeWords(context);
    }

    private void FixMixedBaselineWords(IPipelineContext context, double minBaseLineDiff)
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
        page.Words.Remove(word);
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
            page.Words.Add(newWord);
        }
    }

    private void FixMixedLetterTypeWords(IPipelineContext context)
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
        page.Words.Remove(word);
        
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
            page.Words.Add(firstWord);
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
        page.Words.Add(numbersWord);
            
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
            page.Words.Add(lastWord);
        }



        // var othersBaseLineAverage = firstPart.Union(lastPart).Average(l => l.StartBaseLine.Y);
        // var numberBaseLineAvg = numbersPart.Average(l => l.StartBaseLine.Y);
        //
        // if (Math.Abs(othersBaseLineAverage - numberBaseLineAvg) > 1.5)
        // {
        //     return;
        // }
        
        
        // new word
        // var newWord = word.Word.Clone();
        // newWord.ChangeWord(allLetters.Take(pref.Length).ToList());
        // wordsToInsert.Add(i, newWord);
        // // this word
        // word.ChangeWord(allLetters.Skip(pref.Length).ToList());
    }

    private void RemoveBigSpaces(IPipelineContext context, CleanWordsStepSettings settings)
    {
        var removedTotal = 0;
        foreach (var page in context.Document!.Pages)
        {
            var removedWords = page.Words.RemoveAll(w =>
                string.IsNullOrWhiteSpace(w.Text) && w.Letters.Count == 1 && w.Letters.Any(l => l.PointSize > settings.BigSpacesSize));
            removedTotal += removedWords;
        }
        
        if (removedTotal > 0)
        {
            _log.Write(EnumLogLevel.Debug, $"Big spaces removed: {removedTotal}");
        }
    }

    //     /// <summary>
    // /// check "special" words
    // /// </summary>
    // /// <param name="pdfDoc"></param>
    // /// <param name="footNoteMaxSize"></param>
    // public void CleanWords(IPdfDoc pdfDoc, double footNoteMaxSize)
    // {
    //     var prefixes = FedlexStructure.BisTerQuater.Split("|");
    //     var suffixes = new[] { ")", ",", ";", "]" };
    //     var cc = CultureInfo.CurrentCulture;
    //
    //     foreach (var page in pdfDoc.Pages)
    //     {
    //         foreach (var block in page.Blocks)
    //         {
    //             foreach (var line in block.Lines)
    //             {
    //                 var wordsToInsert = new Dictionary<int, IWordOnPage>();
    //                 for (var i = 0; i < line.Count; i++)
    //                 {
    //                     var word = line[i];
    //
    //                     // check if text is small enough (with some tolerance)
    //                     if (word.BoundingBox.Height >= footNoteMaxSize + 0.3)
    //                     {
    //                         continue;
    //                     }
    //
    //                     // check if small word starts with bis, ter, quater, ...
    //                     // when this is followed by a footnote, it's possible that the word 
    //                     // was detected like "bis375". therefore, if any word starts with this,
    //                     // it has to be fixed
    //                     if (prefixes.Any(pref => word.Text.StartsWith(pref, false, cc)))
    //                     {
    //                         var ma = _inlineFootnoteBisTerRx.Match(word.Text);
    //                         if (!ma.Success)
    //                         {
    //                             continue;
    //                         }
    //
    //                         var pref = ma.Groups["bister"].Value;
    //                         var allLetters = word.Letters.ToList();
    //                         // new word
    //                         var newWord = word.Clone();
    //                         newWord.ChangeWord(allLetters.Take(pref.Length).ToList());
    //                         wordsToInsert.Add(i, newWord);
    //                         // this word
    //                         word.ChangeWord(allLetters.Skip(pref.Length).ToList());
    //                     }
    //
    //                     // remove any dots, commas, brackets and so on
    //                     if (word.Text.Length > 1 && suffixes.Any(sf => word.Text.EndsWith(sf, false, cc)))
    //                     {
    //                         var suff = word.Letters.Last();
    //                         var newWord = word.Clone();
    //                         // var shift = word.BoundingBox.Width - 
    //                         newWord.ChangeWord(new List<Letter> { suff });
    //                         wordsToInsert.Add(i + 1, newWord);
    //                         word.ChangeWord(word.Letters.Take(word.Letters.Count - 1).ToList());
    //                     }
    //                 }
    //
    //                 // insert new words, if necessary
    //                 foreach (var idx in wordsToInsert.Keys)
    //                 {
    //                     line.Insert(idx, wordsToInsert[idx]);
    //                 }
    //             }
    //         }
    //     }
    // }
}
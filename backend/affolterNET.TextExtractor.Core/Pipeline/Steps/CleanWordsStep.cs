using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class CleanWordsStep: IPipelineStep
{
    private readonly IWordCleaner _wordCleaner;
    private readonly IOutput _log;

    public CleanWordsStep(IWordCleaner wordCleaner, IOutput log)
    {
        _wordCleaner = wordCleaner;
        _log = log;
    }
    
    public class CleanWordsStepSettings: IStepSettings
    {
        public double BigSpacesSize { get; set; } = 100;
        public double MinBaseLineDiff { get; set; } = 1;
        public char[] StartPunctuationChars { get; set; } = ['[', '('];
        public char[] EndPunctuationChars { get; set; } = [' ', ',', '.', ']', ')'];
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Cleaning words");
        var settings = context.GetSettings<CleanWordsStepSettings>();
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }

        var w = context.Document.Words.Where(w => w.Text.Length > 1 && w.Text.StartsWith(" ")).ToList();
        if (w.Any())
        {
            _log.Write(EnumLogLevel.Warning, $"{w.Count} words starting with space detected");
        }
        
        // remove useless big spaces (Fedlex-Laws)
        _wordCleaner.RemoveBigSpaces(context, settings);
        
        // separate words containing letters with different baselines
        _wordCleaner.FixMixedBaselineWords(context, settings.MinBaseLineDiff);
        
        // separate words with numbers and other signs
        // not set at the moment bc. of stuff like "m3".
        // _wordCleaner.FixMixedLetterTypeWords(context);
        
        // separate words that contain punctuation
        _wordCleaner.SeparatePunctuation(context, settings.StartPunctuationChars, settings.EndPunctuationChars);
    }
}
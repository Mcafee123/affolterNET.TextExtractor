using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class DetectFootnotesStep: IPipelineStep
{
    private readonly IFootnoteDetector _footnoteDetector;
    private readonly IOutput _log;

    public DetectFootnotesStep(IFootnoteDetector footnoteDetector, IOutput log)
    {
        _footnoteDetector = footnoteDetector;
        _log = log;
    }
    
    public class DetectFootnotesStepSettings: IStepSettings
    {
    
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Detecting footnotes");
        var cleanWordsSettings = context.GetSettings<CleanWordsStep.CleanWordsStepSettings>();
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadWordsStep)} before this step");
        }
        
        if (context.Document.FontSizes == null)
        {
            throw new NullReferenceException(
                $"context.Document.FontSizes not initialized. Run {nameof(AnalyzeLineSpacingStep)} before this step");
        }
        
        var fontSizes = context.Document.FontSizes;
        var mainFontSize = (double)9;
        var mainFontSetting = fontSizes.ToList().MaxBy(fs => fs.WordCount);
        if (mainFontSetting != null)
        {
            mainFontSize = mainFontSetting.AvgFontSize;
        }
        
        foreach (var page in context.Document.Pages)
        {
            foreach (var block in page.Blocks)
            {
                var footnotes = _footnoteDetector.DetectInlineFootnotes(block, mainFontSize, cleanWordsSettings.MinBaseLineDiff);
                context.Document.Footnotes.AddRange(footnotes);
            }
        }

        foreach (var page in context.Document.Pages)
        {
            var footNotesWithoutInlineWord = _footnoteDetector.DetectBottomFootnotes(page, context.Document.Footnotes, mainFontSize);
            context.Document.FootnotesWithoutInlineWords.AddRange(footNotesWithoutInlineWord);
        }

        _log.Write(EnumLogLevel.Info, $"Detected {context.Document.Footnotes.Count} footnotes");
    }
}
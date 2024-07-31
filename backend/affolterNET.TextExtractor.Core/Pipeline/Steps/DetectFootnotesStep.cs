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
        public double MaxDistFromLeft { get; set; } = 1;
        public double LeftBorderGroupRange { get; set; } = 10;
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Detecting footnotes");
        var settings = context.GetSettings<DetectFootnotesStepSettings>();
        var cleanWordsSettings = context.GetSettings<CleanWordsStep.CleanWordsStepSettings>();
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }
        if (context.Document.FontSizes == null)
        {
            throw new NullReferenceException(
                $"context.Document.FontSizes not initialized. Run {nameof(AnalyzeLineSpacingStep)} before this step");
        }

        _footnoteDetector.DetectFootnotes(context.Document, cleanWordsSettings.MinBaseLineDiff);
        _log.Write(EnumLogLevel.Info, $"Detected {context.Document.Footnotes.Count} footnotes");
    }
}
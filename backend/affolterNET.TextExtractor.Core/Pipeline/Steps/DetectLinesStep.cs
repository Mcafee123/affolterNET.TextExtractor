using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class DetectLinesStep: IPipelineStep
{
    private readonly ILineDetector _lineDetector;
    private readonly IOutput _log;

    public DetectLinesStep(ILineDetector lineDetector, IOutput log)
    {
        _lineDetector = lineDetector;
        _log = log;
    }
    
    public class DetectLinesStepSettings: IStepSettings
    {
        public int MaxPagesToConsider { get; set; } = int.MaxValue;
        public double BaseLineMatchingRange { get; set; } = 0.2;
    }
    
    public void Execute(IPipelineContext context)
    {
        var settings = context.GetSettings<DetectLinesStepSettings>();
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadWordsStep)} before this step");
        }
        foreach (var page in context.Document.Pages)
        {
            var lines = _lineDetector.DetectLines(page.Words, settings.MaxPagesToConsider, settings.BaseLineMatchingRange);
            page.Lines.AddRange(lines.ToList());
        }

        var linesSum = context.Document.Pages.Sum(p => p.Lines.Count);
        _log.Write(EnumLogLevel.Debug, "[yellow]", $"Total Lines: {linesSum}", "[/]");
    }
}

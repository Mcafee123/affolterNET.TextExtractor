using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

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
    }
    
    public void Execute(IPipelineContext context)
    {
        var settings = context.GetSettings<CleanWordsStepSettings>();
        
        // remove useless big spaces (Fedlex-Laws)
        foreach (var page in context.Document!.Pages)
        {
            var removedWords = page.Words.RemoveAll(w =>
                string.IsNullOrWhiteSpace(w.Text) && w.Letters.Count == 1 && w.Letters.Any(l => l.PointSize > settings.BigSpacesSize));
            if (removedWords > 0)
            {
                _log.Write(EnumLogLevel.Debug, "[yellow]", $"Big spaces removed on page {page.Nr}: {removedWords}", "[/]");
            }
        }
    }
}
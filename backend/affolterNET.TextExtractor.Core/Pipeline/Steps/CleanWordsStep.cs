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
    
    public void Execute(IPipelineContext context)
    {
        // remove useless big spaces (Fedlex-Laws)
        foreach (var page in context.Document!.Pages)
        {
            var removedWords = page.Words.RemoveAll(w =>
                string.IsNullOrWhiteSpace(w.Text) && w.Letters.Count == 1 && w.Letters.Any(l => l.PointSize > context.BigSpacesSize));
            if (removedWords > 0)
            {
                _log.Write(EnumLogLevel.Debug, "[yellow]", $"Big spaces removed on page {page.Nr}: {removedWords}", "[/]");
            }
        }
    }
}
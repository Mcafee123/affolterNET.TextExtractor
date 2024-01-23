using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Util;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class ReadWordsStep: IPipelineStep
{
    private readonly IWordExtractor _wordExtractor;
    private readonly IOutput _log;

    public ReadWordsStep(IWordExtractor wordExtractor, IOutput log)
    {
        _wordExtractor = wordExtractor;
        _log = log;
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Warning, "[blue]", $"Parsing PDF: {context.Filename}", "[/]");
        // open
        Open(context);
        // get pages
        context.Document!.GetPages();
        // read pages
        ReadPages(context);
        _log.Write(EnumLogLevel.Debug, "[yellow]", $"Pages: {context.Document.Pages.Count}", "[/]");
    }

    private void Open(IPipelineContext context)
    {
        var document = PdfDocument.Open(context.Filename);
        context.SetDocument(document);
    }

    private void ReadPages(IPipelineContext context)
    {
        context.OriginalWords.Clear();
        foreach (var page in context.Document!.Pages)
        {
            var words = GetWords(page);
            context.OriginalWords.AddRange(words);

            foreach (var word in words)
            {
                var wop = new WordOnPage(page.Nr, word);
                page.Words.Add(wop);
            }
        }
    }

    private List<Word> GetWords(IPdfPage page)
    {
        return page.Page.GetWords(_wordExtractor).ToList();
    }
}
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Util;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class ReadWordsStep : IPipelineStep
{
    private readonly IWordExtractor _wordExtractor;
    private readonly IOutput _log;

    public ReadWordsStep(IWordExtractor wordExtractor, IOutput log)
    {
        _wordExtractor = wordExtractor;
        _log = log;
    }
    
    public class ReadWordsStepSettings: IStepSettings
    {
        public double BaseLineGroupRange { get; set; } = 0.3;
    }

    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Warning, "[blue]", $"Parsing PDF: {context.Filename}", "[/]");
        _log.Write(EnumLogLevel.Info, "Reading words");
        // open
        Open(context);
        // get pages
        context.Document!.GetPages();
        // read pages
        ReadPages(context);
        _log.Write(EnumLogLevel.Debug, $"Pages: {context.Document.Pages.Count}");
    }

    private void Open(IPipelineContext context)
    {
        PdfDocument document;
        if (context.PdfStream != null)
        {
            context.PdfStream.Seek(0, SeekOrigin.Begin);
            document = PdfDocument.Open(context.PdfStream);
        }
        else
        {
            document = PdfDocument.Open(context.Filename);
        }

        context.SetDocument(document);
    }

    private void ReadPages(IPipelineContext context)
    {
        var settings = context.GetSettings<ReadWordsStepSettings>();
        context.OriginalWords.Clear();
        foreach (var page in context.Document!.Pages)
        {
            var words = GetWords(page);
            context.OriginalWords.AddRange(words);

            foreach (var word in words)
            {
                var wop = new WordOnPage(page.Nr, word, settings.BaseLineGroupRange);
                page.Words.Add(wop);
            }
        }
    }

    private List<Word> GetWords(IPdfPage page)
    {
        return page.Page.GetWords(_wordExtractor).ToList();
    }
}
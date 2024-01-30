using affolterNET.TextExtractor.Core.Extensions;
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
        _log.Write(EnumLogLevel.Debug, $"Pages: {context.Document.Pages.Count}");
        // add font-statistics
        GetStatistics(context);
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

    private void GetStatistics(IPipelineContext context)
    {
        var fontSizes = context.Document!.Words.FindCommonGroups<IWordOnPage>(1, w => w.FontSizeAvg);
        context.Document.MainFontSizeAvg = fontSizes.First().Average(kvp => kvp.Item1);
    }
}
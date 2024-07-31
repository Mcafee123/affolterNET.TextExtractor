using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Util;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class ReadPagesStep : IPipelineStep
{
    private readonly IWordExtractor _wordExtractor;
    private readonly IOutput _log;

    public ReadPagesStep(IWordExtractor wordExtractor, IOutput log)
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
        context.OriginalImages.Clear();
        foreach (var page in context.Document!.Pages)
        {
            var mainBlock = new PdfTextBlock(page);
            page.Blocks.Add(mainBlock);
            var words = GetWords(page);
            context.OriginalWords.AddRange(words);
            var wordList = new List<IWordOnPage>();
            foreach (var word in words)
            {
                var wop = new WordOnPage(page.Nr, word, settings.BaseLineGroupRange);
                wordList.Add(wop);
            }
            mainBlock.AddWords(wordList.ToArray());
            
            var images = GetImages(page);
            context.OriginalImages.AddRange(images);
            foreach (var image in images)
            {
                var ib = new PdfImageBlock(page, image);
                page.Blocks.Add(ib);
            }
        }
    }

    private List<Word> GetWords(IPdfPage page)
    {
        return page.Page.GetWords(_wordExtractor).ToList();
    }
    
    private List<IPdfImage> GetImages(IPdfPage page)
    {
        return page.Page.GetImages().ToList();
    }
}
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class PdfFootnoteJson
{
    public PdfFootnoteJson()
    {
        
    }

    public PdfFootnoteJson(Footnote footnote, IOutput log)
    {
        Id = footnote.Id;
        foreach (var w in footnote.InlineWords)
        {
            InlineWords.Add(new WordOnPageJson(w, log));
        }

        BottomContents = new PdfTextBlockJson(footnote.BottomContents, log);
        if (footnote.BottomContentsCaption != null)
        {
            BottomContentsCaption = new PdfLineJson(footnote.BottomContentsCaption, log);
        }
        else
        {
            log.Write(EnumLogLevel.Error, $"Footnote {footnote.Id}: BottomContentsCaption is null, page: {InlineWords.FirstOrDefault()?.PageNr}");
        }
    }

    public string Id { get; set; } = null!;
    public List<WordOnPageJson> InlineWords { get; set; } = new();
    public PdfTextBlockJson BottomContents { get; set; } = new();
    public PdfLineJson? BottomContentsCaption { get; set; }
}
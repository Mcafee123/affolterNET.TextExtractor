using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using UglyToad.PdfPig;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfDoc : IPdfDoc
{
    private readonly PdfDocument _document;

    public PdfDoc(string fileName, PdfDocument document)
    {
        Filename = fileName;
        _document = document;
    }
    
    public string Filename { get; }
    public List<IPdfPage> Pages { get; set; } = new();
    public List<IWordOnPage> Words => Pages.SelectMany(p => p.Words).ToList();
    public List<Footnote> Footnotes { get; set; } = new();
    public List<Footnote> FootnotesWithoutInlineWords { get; set; } = new();
    public FontSizeSettings? FontSizes { get; set; }

    public void GetPages()
    {
        Pages.Clear();
        var pages = _document.GetPages().ToList();
        foreach (var page in pages)
        {
            var pg = new PdfPage(page);
            Pages.Add(pg);
        }
    }
    
    public void ToJson(string path, IOutput log)
    {
        this.Serialize(path, log);
    }

    public void Dispose()
    {
        _document.Dispose();
    }
}

public interface IPdfDoc : IDisposable
{
    string Filename { get; }
    List<IPdfPage> Pages { get; set; }
    List<IWordOnPage> Words { get; }
    FontSizeSettings? FontSizes { get; set; }
    List<Footnote> Footnotes { get; set; }
    List<Footnote> FootnotesWithoutInlineWords { get; set; }
    void GetPages();
    void ToJson(string path, IOutput log);
}
using affolterNET.TextExtractor.Core.Extensions;
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
    
    public void ToJson(string path)
    {
        this.Serialize(path);
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
    void GetPages();
    void ToJson(string path);
}
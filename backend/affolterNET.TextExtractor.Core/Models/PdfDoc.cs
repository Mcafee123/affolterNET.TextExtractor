using UglyToad.PdfPig;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfDoc : IPdfDoc
{
    private readonly PdfDocument _document;

    public PdfDoc(PdfDocument document)
    {
        _document = document;
    }

    public void GetPages()
    {
        Pages.Clear();
        var pages = _document!.GetPages().ToList();
        foreach (var page in pages)
        {
            var pg = new PdfPage(page);
            Pages.Add(pg);
        }
    }

    public List<IPdfPage> Pages { get; set; } = new();

    public void Dispose()
    {
        _document.Dispose();
    }
}

public interface IPdfDoc: IDisposable
{
    List<IPdfPage> Pages { get; set; }
    void GetPages();
}
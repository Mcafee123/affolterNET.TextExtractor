using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfPage : IPdfPage
{
    public PdfPage(Page page)
    {
        Page = page;
    }

    public Page Page { get; }
    public PdfRectangle BoundingBox => Page.CropBox.Bounds;
    public int Nr => Page.Number;
    public List<IWordOnPage> Words { get; } = new();
    public PdfLines Lines { get; set; } = new();
    public PdfTextBlocks Blocks { get; set; } = new();
}

public interface IPdfPage
{
    Page Page { get; }
    PdfRectangle BoundingBox { get; }
    int Nr { get; }
    List<IWordOnPage> Words { get; }
    PdfLines Lines { get; set; }
    PdfTextBlocks Blocks { get; set; }
}

using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfPage : IPdfPage
{
    private List<IWordOnPage> _words = new();

    public PdfPage(Page page)
    {
        Page = page;
    }

    public Page Page { get; }
    public PdfRectangle BoundingBox => Page.CropBox.Bounds;
    public int Nr => Page.Number;
    public List<IWordOnPage> Words {
        get
        {
            if (Blocks.Count > 0)
            {
                return Blocks.SelectMany(b => b.Words).ToList();
            }

            if (Lines.Count > 0)
            {
                return Lines.SelectMany(l => l).ToList();
            }

            return _words;
        }
    }
    public PdfLines Lines { get; } = new();
    public PdfTextBlocks Blocks { get; set; } = new();
}

public interface IPdfPage
{
    Page Page { get; }
    PdfRectangle BoundingBox { get; }
    int Nr { get; }
    List<IWordOnPage> Words { get; }
    PdfLines Lines { get; }
    PdfTextBlocks Blocks { get; set; }
}

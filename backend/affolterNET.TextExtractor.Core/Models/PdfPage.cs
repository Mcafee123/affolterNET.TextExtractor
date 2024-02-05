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
    public IEnumerable<IWordOnPage> Words {
        get
        {
            if (Blocks.Count  > 0)
            {
                return Blocks.SelectMany(b => b.Words).ToList();
            }
            if (Lines.Count > 0)
            {
                return Lines.Words.ToList();
            }

            return _words;
        }
    }
    public PdfLines Lines { get; } = new();
    public PdfTextBlocks Blocks { get; set; } = new();

    public void AddWord(IWordOnPage word)
    {
        _words.Add(word);
    }

    public bool RemoveWord(IWordOnPage word)
    {
        return _words.Remove(word);
    }
}

public interface IPdfPage
{
    Page Page { get; }
    PdfRectangle BoundingBox { get; }
    int Nr { get; }
    IEnumerable<IWordOnPage> Words { get; }
    PdfLines Lines { get; }
    PdfTextBlocks Blocks { get; set; }
    void AddWord(IWordOnPage word);
    bool RemoveWord(IWordOnPage word);
}

using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfPage
{
    Page Page { get; }
    PdfRectangle BoundingBox { get; }
    int Nr { get; }
    IEnumerable<IWordOnPage> Words { get; }
    PdfLines Lines { get; }
    PdfBlocks Blocks { get; set; }
    void AddWord(IWordOnPage word);
    bool RemoveWord(IWordOnPage word);
    bool VerifyBlocks(out string message);
}
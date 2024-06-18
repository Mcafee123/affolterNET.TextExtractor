using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfTextBlock: IPdfBlock
{
    int Id { get; }
    List<IWordOnPage> Words { get; }
    PdfLines Lines { get; }
    LineOnPage? FirstLine { get; }
    double FontSizeAvg { get; }
    List<PdfRectangle> VerticalGaps { get; set; }
    List<PdfRectangle> HorizontalGaps { get; set; }
    BlockNodes BlockNodes { get; set; }
    bool Any(Func<LineOnPage, bool> predicate);
    void AddLine(LineOnPage line);
    void AddLines(List<LineOnPage> lines);
    string GetText(Func<IWordOnPage, bool> exclude);
    void RemoveWord(IWordOnPage word);
}
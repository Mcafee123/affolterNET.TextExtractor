using affolterNET.TextExtractor.Core.Services.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfTextBlock: IPdfBlock
{
    int Id { get; }
    IEnumerable<IWordOnPage> Words { get; }
    PdfLines Lines { get; }
    LineOnPage? FirstLine { get; }
    double FontSizeAvg { get; }
    List<PdfRectangle> VerticalGaps { get; set; }
    List<PdfRectangle> HorizontalGaps { get; set; }
    BlockNodes? BlockNodes { get; set; }
    bool Any(Func<LineOnPage, bool> predicate);
    void AddWords(params IWordOnPage[] words);
    void AddLines(params LineOnPage[] lines);
    string GetText(Func<IWordOnPage, bool> exclude);
    void RemoveWord(IWordOnPage word);
    FontSizeSettings? FontSizes { get; }
    void DetectLines(ILineDetector lineDetector, double baseLineMatchingRange);
}
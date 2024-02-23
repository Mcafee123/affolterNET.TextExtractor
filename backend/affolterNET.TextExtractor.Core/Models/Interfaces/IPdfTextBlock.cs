namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfTextBlock: IPdfBlock
{
    int Id { get; }
    List<IWordOnPage> Words { get; }
    PdfLines Lines { get; }
    LineOnPage? FirstLine { get; }
    List<Gap> VerticalGaps { get; set; }
    bool Any(Func<LineOnPage, bool> predicate);
    void AddLine(LineOnPage line);
    void AddLines(List<LineOnPage> lines);
    string GetText(Func<IWordOnPage, bool> exclude);
}
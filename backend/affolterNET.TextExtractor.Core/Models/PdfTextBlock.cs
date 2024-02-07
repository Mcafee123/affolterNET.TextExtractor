using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfTextBlock: IPdfTextBlock
{
    private PdfLines _lines = new();

    public PdfTextBlock(IPdfPage page)
    {
        Page = page;
    }

    public List<IWordOnPage> Words => _lines.Words.ToList();
    public PdfLines Lines => _lines;
    public double TopDistance { get; set; } = LineOnPage.DefaultTopDistance;
    public IPdfPage Page { get; }
    public PdfRectangle BoundingBox => _lines.BoundingBox;
    public LineOnPage? FirstLine => _lines.FirstOrDefault();
    public List<Gap> VerticalGaps { get; set; } = new();
    public void AddLines(PdfLines lines)
    {
        _lines = lines;
    }
    
    public void AddLines(List<LineOnPage> lines)
    {
        _lines.AddRange(lines);
    }
    
    public void AddLine(LineOnPage line)
    {
        _lines.AddRange(new List<LineOnPage> { line });
    }

    public bool RemoveLine(LineOnPage line)
    {
        return _lines.Remove(line);
    }

    public bool Any(Func<LineOnPage, bool> predicate)
    {
        return _lines.Any(predicate);
    }

    public override string ToString()
    {
        return _lines.ToString();
    }
}
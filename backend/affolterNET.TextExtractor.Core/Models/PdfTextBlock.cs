using affolterNET.TextExtractor.Core.Extensions;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfTextBlock: IPdfTextBlock
{
    private PdfLines _lines = new();
    public List<IWordOnPage> Words => _lines.Words.ToList();
    public PdfLines Lines => _lines;
    public double TopDistance { get; set; } = LineOnPage.DefaultTopDistance;
    public double AverageDistance => _lines.Count < 2 ? 0 : _lines.Skip(1).Select(l => _lines.GetTopDistance(l)).Average();
    public IPdfPage? Page { get; set; }
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

public interface IPdfTextBlock
{
    double TopDistance { get; set; }
    double AverageDistance { get; }
    List<IWordOnPage> Words { get; }
    PdfRectangle BoundingBox { get; }
    IPdfPage? Page { get; set; }
    PdfLines Lines { get; }
    LineOnPage? FirstLine { get; }
    List<Gap> VerticalGaps { get; set; }
    bool Any(Func<LineOnPage, bool> predicate);
    void AddLine(LineOnPage line);
    void AddLines(List<LineOnPage> lines);
}
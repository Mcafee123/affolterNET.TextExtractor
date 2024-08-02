using System.Diagnostics;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfTextBlock: IPdfTextBlock
{
    private static int _blockIdx = 0;
    protected PdfLines _lines = new();
    private List<IWordOnPage> _words = new();

    public PdfTextBlock(IPdfPage page)
    {
        Page = page;
        Id = _blockIdx++;
    }

    public int Id { get; protected set; }
    public IEnumerable<IWordOnPage> Words => _words;
    public PdfLines Lines => _lines;
    public double TopDistance { get; set; } = LineOnPage.DefaultTopDistance;
    public IPdfPage Page { get; }
    public int PageNr => Page.Nr;
    public PdfRectangle BoundingBox { get; private set; } = new(0, 0, 0, 0);
    public LineOnPage? FirstLine => _lines.FirstOrDefault();
    public double FontSizeAvg => Words.Average(w => w.FontSizeAvg);
    public List<PdfRectangle> VerticalGaps { get; set; } = new();
    public List<PdfRectangle> HorizontalGaps { get; set; } = new();
    public BlockNodes? BlockNodes { get; set; }
    public FontSizeSettings? FontSizes { get; set; }
    public double SpaceDistance => _lines.WordSpaceAvg ?? GetSpaceFromFontSize();

    public void AddWords(params IWordOnPage[] words)
    {
        _words.AddRange(words);
        if (_lines.Count > 0)
        {
            Debug.WriteLine($"Words added to block. LINES CLEARED!");
        }
        _lines.Clear();
        Refresh();
    }
    
    public void RemoveWord(IWordOnPage word)
    {
        _words.Remove(word);
        if (_lines.Count > 0)
        {
            Debug.WriteLine($"Word removed from block: {word.Text}. LINES CLEARED!");
        }
        _lines.Clear();
        Refresh();
    }
    
    public void AddLines(params LineOnPage[] lines)
    {
        _lines.AddRange(lines.ToList());
        _words = _lines.SelectMany(l => l).ToList();
        Refresh();
    }
    
    public void AddLine(LineOnPage line)
    {
        _lines.AddRange([line]);
        _words = _lines.SelectMany(l => l).ToList();
        Refresh();
    }

    public bool RemoveLine(LineOnPage line)
    {
        var result = _lines.Remove(line);
        _words = _lines.SelectMany(l => l).ToList();
        Refresh();
        return result;
    }

    public void DetectLines(ILineDetector lineDetector, double baseLineMatchingRange)
    {
        _lines.Clear();
        var lines = lineDetector.DetectLines(_words, baseLineMatchingRange);
        _lines.AddRange(lines.ToList());
    }

    public bool Any(Func<LineOnPage, bool> predicate)
    {
        return _lines.Any(predicate);
    }

    public string GetText(Func<IWordOnPage, bool> exclude)
    {
        return _lines.GetText(exclude, "");
    }

    public override string ToString()
    {
        return _lines.GetText(null, "");
    }
    
    private void Refresh()
    {
        _words = _words.Count > 0
            ? _words.OrderBy(w => w.PageNr).ThenByDescending(w => w.BaseLineY).ToList()
            : _words;
        var top = _words.Count > 0 ? _words.Max(w => w.BoundingBox.Top) : 0;
        var left = _words.Count > 0 ? _words.Min(w => w.BoundingBox.Left) : 0;
        var bottom = _words.Count > 0 ? _words.Min(w => w.BoundingBox.Bottom) : 0;
        var right = _words.Count > 0 ? _words.Max(w => w.BoundingBox.Right) : 0;
        var bottomLeft = new PdfPoint(left, bottom);
        var topRight = new PdfPoint(right, top);
        BoundingBox = new PdfRectangle(bottomLeft, topRight);
        FontSizes = new FontSizeSettings(_words);
    }

    private double GetSpaceFromFontSize()
    {
        var fontSize = FontSizes?.MaxBy(fs => fs.MaxFontSize);
        if (fontSize == null)
        {
            throw new InvalidOperationException("no font size found");
        }

        return fontSize.MaxFontSize / 2;
    }
}
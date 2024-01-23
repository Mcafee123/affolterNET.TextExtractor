using System.Collections;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class LineOnPage : IList<IWordOnPage>, ILineOnPage
{
    private List<IWordOnPage> _words = new();

    public const double DefaultTopDistance = 2000;

    public LineOnPage(List<IWordOnPage> words, int pageNr)
    {
        PageNr = pageNr;
        AddRange(words);
    }

    public Gap TopDistance { get; set; } = new Gap(DefaultTopDistance);
    public PdfRectangle BoundingBox { get; private set; }
    public int Count => _words.Count;

    public int CountWithoutSpaces()
    {
        return _words.Count(w => w.HasText);
    }

    public bool IsReadOnly => false;
    public int PageNr { get; set; }
    public double Top { get; set; }

    public double Left { get; set; }

    public double Bottom { get; set; }

    public double Right { get; set; }
    public PdfLines? Lines { get; set; }
    public IWordOnPage? FirstWord => _words.FirstOrDefault();

    public TextOrientation TextOrientation { get; private set; }

    public void AddRange(List<IWordOnPage> items)
    {
        items.ForEach(item => item.Line = this);
        _words.AddRange(items);
        var orientations = _words.Select(w => w.TextOrientation).Distinct().ToList();
        if (orientations.Count > 1)
        {
            throw new InvalidOperationException(
                $"multiple orientations found: {string.Join(" ", orientations.Select(o => o.ToString()))}");
        }

        TextOrientation = _words.Count > 0 ? _words.First().TextOrientation : TextOrientation.Other;
        _words = _words.Count > 0
            ? _words.OrderBy(w =>
                TextOrientation == TextOrientation.Horizontal ? w.BoundingBox.Left : w.BoundingBox.Bottom).ToList()
            : _words;
        Top = _words.Count > 0 ? _words.Max(w => w.BoundingBox.Top) : 0;
        Left = _words.Count > 0 ? _words.Min(w => w.BoundingBox.Left) : 0;
        Bottom = _words.Count > 0 ? _words.Min(w => w.BoundingBox.Bottom) : 0;
        Right = _words.Count > 0 ? _words.Max(w => w.BoundingBox.Right) : 0;
        var bottomLeft = new PdfPoint(Left, Bottom);
        var topRight = new PdfPoint(Right, Top);
        BoundingBox = new PdfRectangle(bottomLeft, topRight);
    }

    public void Add(IWordOnPage item)
    {
        AddRange(new List<IWordOnPage> { item });
    }
    
    public override string ToString()
    {
        return string.Join("", _words.Select(w => w.Text)).Trim();
    }

    public IEnumerator<IWordOnPage> GetEnumerator()
    {
        return _words.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        _words.Clear();
    }

    public bool Contains(IWordOnPage item)
    {
        return _words.Contains(item);
    }

    public void CopyTo(IWordOnPage[] array, int arrayIndex)
    {
        _words.CopyTo(array, arrayIndex);
    }

    public bool Remove(IWordOnPage item)
    {
        return _words.Remove(item);
    }

    public int IndexOf(IWordOnPage item)
    {
        return _words.IndexOf(item);
    }

    public void Insert(int index, IWordOnPage item)
    {
        _words.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _words.RemoveAt(index);
    }

    public IWordOnPage this[int index]
    {
        get => _words[index];
        set => _words[index] = value;
    }
}

public interface ILineOnPage
{
    PdfRectangle BoundingBox { get; }
    double Left { get; }
    double Right { get; }
}
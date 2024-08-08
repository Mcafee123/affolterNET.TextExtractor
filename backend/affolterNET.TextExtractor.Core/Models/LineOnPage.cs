using System.Collections;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class LineOnPage : IList<IWordOnPage>, ILineOnPage
{
    private static long _id = 0;
    private List<IWordOnPage> _words = new();

    public const double DefaultTopDistance = 2000;

    public LineOnPage(List<IWordOnPage> words, int pageNr)
    {
        Id = _id++;
        PageNr = pageNr;
        AddRange(words);
    }

    public long Id { get; }
    public PdfRectangle BoundingBox { get; private set; }
    public int Count => _words.Count;
    public double BaseLineY { get; set; }
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
    public IWordOnPage? FirstWord => _words.FirstOrDefault();
    public IWordOnPage? FirstWordWithText => _words.FirstOrDefault(w => w.HasText);
    public double FontSizeAvg => _words.Average(w => w.FontSizeAvg);
    public double? WordSpaceAvg
    {
        get
        {
            var spaces = _words.Where(w => w.Text == " ").ToList();
            if (spaces.Count < 1)
            {
                // if there are no spaces, calculate the average width of all letters
                var letters = _words.SelectMany(w => w.Letters).ToList();
                if (letters.Count < 1)
                {
                    return null;
                }

                return letters.Average(l => l.Width);
            }

            return spaces.Average(s => s.BoundingBox.Width);
        }
    }
    public TextOrientation TextOrientation { get; private set; }
    public double TopDistance { get; private set; } = DefaultTopDistance;

    public void SetTopDistance(double topDistance)
    {
        TopDistance = topDistance;
    }

    public void AddRange(List<IWordOnPage> items)
    {
        items.ForEach(item => item.Line = this);
        _words.AddRange(items);
        Refresh();
    }

    public void Add(IWordOnPage item)
    {
        item.Line = this;
        _words.Add(item);
        Refresh();
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
        Refresh();
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
        var result = _words.Remove(item);
        Refresh();
        return result;
    }

    public int IndexOf(IWordOnPage item)
    {
        return _words.IndexOf(item);
    }

    public void Insert(int index, IWordOnPage item)
    {
        _words.Insert(index, item);
        Refresh();
    }

    public void RemoveAt(int index)
    {
        _words.RemoveAt(index);
        Refresh();
    }

    public IWordOnPage this[int index]
    {
        get => _words[index];
        set => _words[index] = value;
    }

    public void RemoveAll(IEnumerable<IWordOnPage> words)
    {
        foreach (var word in words)
        {
            _words.Remove(word);
        }

        Refresh();
    }

    public void Refresh()
    {
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
        BaseLineY = _words.Count > 0 ? _words.Average(w => w.BaseLineY) : 0;
    }
}

public interface ILineOnPage
{
    long Id { get; }
    PdfRectangle BoundingBox { get; }
    double Left { get; }
    double Right { get; }
    double BaseLineY { get; set; }
    double FontSizeAvg { get; }
    int PageNr { get; }
    int IndexOf(IWordOnPage item);
    int Count { get; }
    IWordOnPage this[int index] { get; set; }
    IWordOnPage? FirstWord { get; }
}
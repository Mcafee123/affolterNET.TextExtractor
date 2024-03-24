using System.Collections;
using affolterNET.TextExtractor.Core.Extensions;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfLines : IList<LineOnPage>
{
    private List<LineOnPage> _lines = new();

    public PdfLines(params LineOnPage[] lines)
    {
        AddRange(lines.ToList());
    }

    public IEnumerable<IWordOnPage> Words => _lines.SelectMany(line => line);

    public int Count => _lines.Count;
    public double FontSizeAvg => _lines.Average(l => l.FontSizeAvg);
    public double? WordSpaceAvg => _lines.Average(l => l.WordSpaceAvg);
    public bool IsReadOnly => false;
    public PdfRectangle BoundingBox { get; private set; }

    public IEnumerator<LineOnPage> GetEnumerator()
    {
        return _lines.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddRange(List<LineOnPage> items)
    {
        // filter out empty lines
        items.RemoveAll(l => l.All(w => string.IsNullOrWhiteSpace(w.Text)));
        if (items.Count < 1)
        {
            return;
        }

        items.ForEach(item => item.Lines = this);
        _lines.AddRange(items);
        Refresh();
    }

    public void Add(LineOnPage item)
    {
        AddRange(new List<LineOnPage> { item });
    }

    public string GetText(Func<IWordOnPage, bool>? exclude, string lineSeparator)
    {
        return string.Join(lineSeparator, GetLines(exclude));
    }

    public List<string> GetLines(Func<IWordOnPage, bool>? exclude)
    {
        var lineList = new List<string>();
        // exclude lines based on word lists
        if (exclude != null && _lines.SelectMany(l => l).All(exclude))
        {
            return lineList;
        }

        // get line-words, exclude words
        var stringLines = _lines
            .Select(l => l.Where(w => exclude == null || !exclude(w)).Select(w => w.Text).ToList())
            .ToList();

        // fix hyphenation
        for (var i = 0; i < stringLines.Count; i++)
        {
            var words = stringLines[i];
            if (LastWordOnPrevLineEndsWith(stringLines, i, '-') && FirstWordStartsLowercase(words))
            {
                var wordWithoutDash = stringLines[i - 1][^1];
                stringLines[i - 1][^1] = $"{wordWithoutDash.Substring(0, wordWithoutDash.Length - 1)}{words[0]}";
                stringLines[i].RemoveAt(0);
            }
        }

        return stringLines.Select(w => string.Join("", w)).ToList();
    }


    public override string ToString()
    {
        return GetText(null, "");
    }

    public void Clear()
    {
        _lines.Clear();
        Refresh();
    }

    public bool Contains(LineOnPage item)
    {
        return _lines.Contains(item);
    }

    public void CopyTo(LineOnPage[] array, int arrayIndex)
    {
        _lines.CopyTo(array, arrayIndex);
    }

    public bool Remove(LineOnPage item)
    {
        var result = _lines.Remove(item);
        Refresh();
        return result;
    }

    public int IndexOf(LineOnPage item)
    {
        return _lines.IndexOf(item);
    }

    public void Insert(int index, LineOnPage item)
    {
        _lines.Insert(index, item);
        Refresh();
    }

    public void RemoveAt(int index)
    {
        _lines.RemoveAt(index);
        Refresh();
    }

    public void RemoveAll(IEnumerable<LineOnPage> lines)
    {
        foreach (var line in lines)
        {
            _lines.Remove(line);
        }

        Refresh();
    }

    public LineOnPage this[int index]
    {
        get => _lines[index];
        set => _lines[index] = value;
    }

    public bool ContainsWord(IWordOnPage word)
    {
        var words = _lines.SelectMany(l => l).ToList();
        return words.Contains(word);
    }

    public ILineOnPage? FindLineOnTop(ILineOnPage line)
    {
        var boxCurrent = line.BoundingBox;
        var lineOnTop = _lines
            .Where(l => l.PageNr == line.PageNr
                        && l.BoundingBox.OverlapsX(boxCurrent)
                        && l.BoundingBox.Centroid.Y > line.BoundingBox.Centroid.Y
                        && !l.BoundingBox.OverlapsY(line.BoundingBox))
            .MinBy(l => Math.Abs(l.BoundingBox.Centroid.Y - line.BoundingBox.Centroid.Y));
        return lineOnTop;
    }

    public void SetTopDistance(LineOnPage line)
    {
        var lineOnTop = FindLineOnTop(line);
        var topDistance = line.GetTopDistance(lineOnTop);
        line.SetTopDistance(topDistance);
    }

    private bool FirstWordStartsLowercase(List<string> words)
    {
        var firstWordFirstChar = (words.FirstOrDefault() ?? "X").First();
        return char.IsLower(firstWordFirstChar);
    }

    private bool LastWordOnPrevLineEndsWith(List<List<string>> stringLines, int currentLineIndex,
        params char[] endings)
    {
        if (currentLineIndex == 0)
        {
            // this is the first line
            return false;
        }

        var lastWordOnPrevLine = stringLines[currentLineIndex - 1].LastOrDefault();
        if (lastWordOnPrevLine == null)
        {
            // no words on the previous line .. rather theoretical...
            return false;
        }

        var lastWordText = lastWordOnPrevLine.Trim();
        var contained = false;
        if (lastWordText.Length <= 0)
        {
            return contained;
        }

        var lastSign = lastWordText.Last();
        contained = endings.Contains(lastSign);

        return contained;
    }

    private void Refresh()
    {
        _lines = _lines.Count > 0
            ? _lines.OrderBy(line => line.PageNr).ThenByDescending(line => line.BaseLineY).ToList()
            : _lines;
        var top = _lines.Count > 0 ? _lines.Max(w => w.BoundingBox.Top) : 0;
        var left = _lines.Count > 0 ? _lines.Min(w => w.BoundingBox.Left) : 0;
        var bottom = _lines.Count > 0 ? _lines.Min(w => w.BoundingBox.Bottom) : 0;
        var right = _lines.Count > 0 ? _lines.Max(w => w.BoundingBox.Right) : 0;
        var bottomLeft = new PdfPoint(left, bottom);
        var topRight = new PdfPoint(right, top);
        BoundingBox = new PdfRectangle(bottomLeft, topRight);
    }
}
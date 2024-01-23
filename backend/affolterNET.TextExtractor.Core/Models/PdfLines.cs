using System.Collections;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfLines : IList<LineOnPage>
{
    private List<LineOnPage> _lines = new();
    
    public IEnumerable<IWordOnPage> Words => _lines.SelectMany(line => line);

    public int Count => _lines.Count;

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
        _lines = _lines.Count > 0
            ? _lines.OrderBy(line => line.PageNr).ThenByDescending(line => line.Top).ToList()
            : _lines;
        var top = _lines.Count > 0 ? _lines.Max(w => w.BoundingBox.Top) : 0;
        var left = _lines.Count > 0 ? _lines.Min(w => w.BoundingBox.Left) : 0;
        var bottom = _lines.Count > 0 ? _lines.Min(w => w.BoundingBox.Bottom) : 0;
        var right = _lines.Count > 0 ? _lines.Max(w => w.BoundingBox.Right) : 0;
        var bottomLeft = new PdfPoint(left, bottom);
        var topRight = new PdfPoint(right, top);
        BoundingBox = new PdfRectangle(bottomLeft, topRight);
        for (var i = 1; i < _lines.Count; i++)
        {
            var boxCurrent = _lines[i].BoundingBox;
            var boxLast = _lines[i - 1].BoundingBox;
            _lines[i].TopDistance = new Gap(boxLast.Centroid.Y, boxCurrent.Centroid.Y);
        }

        if (_lines.Count > 0)
        {
            _lines[0].TopDistance = new Gap(LineOnPage.DefaultTopDistance);
        }
    }
    
    public void Add(LineOnPage item)
    {
        AddRange(new List<LineOnPage> { item });
    }

    public override string ToString()
    {
        var wordList = new List<string>();
        for (var i = 0; i < _lines.Count; i++)
        {
            var words = _lines[i];
            if (LastWordOnPrevLineEndsWith(i, out _, '-') && FirstWordStartsLowercase(words))
            {
                var wordWithoutDash = wordList[^1];
                wordList[^1] = $"{wordWithoutDash.Substring(0, wordWithoutDash.Length - 1)}{words[0].Text}";
                var nextLine = words.Skip(1);
                wordList.AddRange(nextLine.Select(w => w.Text));
            }
            else
            {
                wordList.AddRange(words.Select(w => w.Text));
            }
        }

        return string.Join("", wordList);
    }

    public void Clear()
    {
        _lines.Clear();
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
        return _lines.Remove(item);
    }

    public int IndexOf(LineOnPage item)
    {
        return _lines.IndexOf(item);
    }

    public void Insert(int index, LineOnPage item)
    {
        _lines.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _lines.RemoveAt(index);
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

    private bool FirstWordStartsLowercase(LineOnPage words)
    {
        var firstWordFirstChar = (words.FirstOrDefault()?.Text ?? "X").First();
        return char.IsLower(firstWordFirstChar);
    }

    private bool LastWordOnPrevLineEndsWith(int currentLineIndex, out IWordOnPage? lastWordOnPrevLine,
        params char[] endings)
    {
        lastWordOnPrevLine = null;
        if (currentLineIndex == 0)
        {
            // this is the first line
            return false;
        }

        lastWordOnPrevLine = _lines[currentLineIndex - 1].LastOrDefault();
        if (lastWordOnPrevLine == null)
        {
            // no words on the previous line .. rather theoretical...
            return false;
        }

        var lastWordText = lastWordOnPrevLine.Text.Trim();
        var contained = false;
        if (lastWordText.Length <= 0)
        {
            return contained;
        }
        var lastSign = lastWordOnPrevLine.Text.Trim().Last();
        contained = endings.Contains(lastSign);

        return contained;
    }
}
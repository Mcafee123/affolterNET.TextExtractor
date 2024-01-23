using System.Collections;
using affolterNET.TextExtractor.Core.Extensions;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfTextBlocks: IPdfTextBlocks
{
    private List<IPdfTextBlock> _blocks = new();

    public PdfTextBlocks()
    {
        
    }
    
    public PdfTextBlocks(PdfTextBlock block)
    {
        AddRange(new List<IPdfTextBlock> { block });        
    }
    
    public IEnumerator<IPdfTextBlock> GetEnumerator()
    {
        return _blocks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(IPdfTextBlock item)
    {
        AddRange(new List<IPdfTextBlock> { item });
    }

    public void AddRange(List<IPdfTextBlock> items)
    {
        _blocks.AddRange(items);
        _blocks = _blocks.Count > 0
            ? _blocks.OrderBy(b => b, Comparer<IPdfTextBlock>.Create((tb1, tb2) =>
                {
                    if (tb1.Page != null && tb2.Page != null && tb1.Page.Nr != tb2.Page.Nr)
                    {
                        return tb1.Page.Nr.CompareTo(tb2.Page.Nr);
                    }
                    if (tb1.BoundingBox.OverlapsY(tb2.BoundingBox))
                    {
                        return tb1.BoundingBox.Left.CompareTo(tb2.BoundingBox.Left);
                    }

                    return tb2.BoundingBox.Top.CompareTo(tb1.BoundingBox.Top);
                }))
                .ToList()
            : _blocks;
        Top = _blocks.Count > 0 ? _blocks.Max(w => w.BoundingBox.Top) : 0;
        Left = _blocks.Count > 0 ? _blocks.Min(w => w.BoundingBox.Left) : 0;
        Bottom = _blocks.Count > 0 ? _blocks.Min(w => w.BoundingBox.Bottom) : 0;
        Right = _blocks.Count > 0 ? _blocks.Max(w => w.BoundingBox.Right) : 0;
        var bottomLeft = new PdfPoint(Left, Bottom);
        var topRight = new PdfPoint(Right, Top);
        BoundingBox = new PdfRectangle(bottomLeft, topRight);
    }

    public PdfRectangle BoundingBox { get; set; }

    public double Right { get; set; }

    public double Bottom { get; set; }

    public double Left { get; set; }

    public double Top { get; set; }

    public void Clear()
    {
        _blocks.Clear();
    }

    public bool Contains(IPdfTextBlock item)
    {
        return _blocks.Contains(item);
    }

    public void CopyTo(IPdfTextBlock[] array, int arrayIndex)
    {
        _blocks.CopyTo(array, arrayIndex);
    }

    public bool Remove(IPdfTextBlock item)
    {
        return _blocks.Remove(item);
    }

    public int Count => _blocks.Count;
    public bool IsReadOnly => false;

    public int IndexOf(IPdfTextBlock item)
    {
        return _blocks.IndexOf(item);
    }

    public void Insert(int index, IPdfTextBlock item)
    {
        _blocks.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _blocks.RemoveAt(index);
    }

    public IPdfTextBlock this[int index]
    {
        get => _blocks[index];
        set => _blocks[index] = value;
    }
}

public interface IPdfTextBlocks : IList<IPdfTextBlock>
{
    void AddRange(List<IPdfTextBlock> items);
}
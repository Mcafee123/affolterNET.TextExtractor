using System.Collections;
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfBlocks: IPdfBlocks
{
    private List<IPdfBlock> _blocks = new();

    public PdfBlocks()
    {
        
    }

    public List<IPdfTextBlock> TextBlocks => _blocks.OfType<IPdfTextBlock>().ToList();
    public List<IPdfImageBlock> ImageBlocks => _blocks.OfType<IPdfImageBlock>().ToList();
    public List<IPdfBlock> GetOverlappingBlocks(out IPdfBlock? block)
    {
        foreach (var b in _blocks)
        {
            var overlappingBlocks = _blocks
                .Where(bl => b != bl && bl.BoundingBox.Overlaps(b.BoundingBox, 0.1)).ToList();
            if (overlappingBlocks.Count > 0)
            {
                block = b;
                return overlappingBlocks;
            }
        }

        block = null;
        return new List<IPdfBlock>();
    }

    public PdfBlocks(IPdfBlock block)
    {
        AddRange([block]);        
    }
    
    public IEnumerator<IPdfBlock> GetEnumerator()
    {
        return _blocks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(IPdfBlock item)
    {
        AddRange([item]);
    }

    public void AddRange(List<IPdfBlock> items)
    {
        _blocks.AddRange(items);
        Refresh();
    }

    public PdfRectangle BoundingBox { get; set; }

    public double Right { get; set; }

    public double Bottom { get; set; }

    public double Left { get; set; }

    public double Top { get; set; }

    public void Clear()
    {
        _blocks.Clear();
        Refresh();
    }

    public bool Contains(IPdfBlock item)
    {
        return _blocks.Contains(item);
    }

    public void CopyTo(IPdfBlock[] array, int arrayIndex)
    {
        _blocks.CopyTo(array, arrayIndex);
    }

    public bool Remove(IPdfBlock item)
    {
        var result = _blocks.Remove(item);
        Refresh();
        return result;
    }

    public int Count => _blocks.Count;
    public bool IsReadOnly => false;

    public int IndexOf(IPdfBlock item)
    {
        return _blocks.IndexOf(item);
    }

    public void Insert(int index, IPdfBlock item)
    {
        _blocks.Insert(index, item);
        Refresh();
    }

    public void RemoveAt(int index)
    {
        _blocks.RemoveAt(index);
        Refresh();
    }

    public IPdfBlock this[int index]
    {
        get => _blocks[index];
        set => _blocks[index] = value;
    }

    public void Refresh()
    {
        _blocks = _blocks.Count > 0
            ? _blocks.OrderBy(b => b, Comparer<IPdfBlock>.Create((tb1, tb2) =>
                {
                    if (tb1.Page.Nr != tb2.Page.Nr)
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
}
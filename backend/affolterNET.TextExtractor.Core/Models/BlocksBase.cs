using System.Collections;
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public abstract class BlocksBase<TBase, TText, TImage>: IEnumerable<TBase> 
    where TText: class, TBase 
    where TImage: class, TBase 
    where TBase: class, IPdfBlockBase
{
    private List<TBase> _blocks = new();

    protected BlocksBase()
    {
        
    }

    public List<TText> TextBlocks => _blocks.OfType<TText>().ToList();
    public List<TImage> ImageBlocks => _blocks.OfType<TImage>().ToList();
    public List<TBase> GetOverlappingBlocks(out TBase? block)
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
        return new List<TBase>();
    }

    protected BlocksBase(TBase block)
    {
        AddRange([block]);        
    }
    
    public IEnumerator<TBase> GetEnumerator()
    {
        return _blocks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual void Add(TBase item)
    {
        AddRange([item]);
    }

    public void AddRange(List<TBase> items)
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

    public bool Contains(TBase item)
    {
        return _blocks.Contains(item);
    }

    public void CopyTo(TBase[] array, int arrayIndex)
    {
        _blocks.CopyTo(array, arrayIndex);
    }

    public bool Remove(TBase item)
    {
        var result = _blocks.Remove(item);
        Refresh();
        return result;
    }

    public int Count => _blocks.Count;
    public bool IsReadOnly => false;

    public int IndexOf(TBase item)
    {
        return _blocks.IndexOf(item);
    }

    public void Insert(int index, TBase item)
    {
        _blocks.Insert(index, item);
        Refresh();
    }

    public void RemoveAt(int index)
    {
        _blocks.RemoveAt(index);
        Refresh();
    }

    public TBase this[int index]
    {
        get => _blocks[index];
        set => _blocks[index] = value;
    }

    public void Refresh()
    {
        _blocks = _blocks.Count > 0
            ? _blocks.OrderBy(b => b, Comparer<TBase>.Create((tb1, tb2) =>
                {
                    if (tb1.PageNr != tb2.PageNr)
                    {
                        return tb1.PageNr.CompareTo(tb2.PageNr);
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
using affolterNET.TextExtractor.Core.Extensions;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class Quadtree
{
    private static int _id = 0;
    
    private int _level;
    private List<PdfRectangle> _objects;
    private readonly double _resolution;
    private Quadtree? nw;
    private Quadtree? ne;
    private Quadtree? sw;
    private Quadtree? se;

    public Quadtree(PdfRectangle boundingBox, double resolution = 1)
    {
        Id = _id;
        _level = 0;
        _objects = new List<PdfRectangle>();
        BoundingBox = boundingBox;
        _resolution = resolution;
        Split();
    }
    
    private Quadtree(int level, PdfRectangle boundingBox, double resolution, Quadtree parent)
    {
        _id++;
        Id = _id;
        _level = level;
        _objects = new List<PdfRectangle>();
        BoundingBox = boundingBox;
        _resolution = resolution;
        Parent = parent;
        Split();
    }

    public int Id { get; set; }
    private PdfRectangle BoundingBox { get; }
    public Quadtree? Parent { get; }
    private bool IsFilled { get; set; }
    private bool IsDeepestLevel => nw == null && ne == null && sw == null && se == null;
    private List<IGrouping<double, Quadtree>> Columns { get; set; } = new();
    private List<IGrouping<double, Quadtree>> Rows { get; set; } = new();
    
    public void Insert(PdfRectangle pRect)
    {
        // if it does not overlap, return
        if (!pRect.Overlaps(BoundingBox))
        {
            return;
        }
        
        // set to filled
        _objects.Add(pRect);
        IsFilled = true;
        if (!IsDeepestLevel)
        {
            nw!.Insert(pRect);
            ne!.Insert(pRect);
            sw!.Insert(pRect);
            se!.Insert(pRect);
        }
    }

    public List<PdfRectangle> GetVerticalGaps()
    {
        GetRowsAndColumns();
        var verticalGaps = GetGaps(Columns);
        var mergedVerticalGaps = new List<PdfRectangle>();
        foreach (var rect in verticalGaps)
        {
            var distToLast = Math.Abs(mergedVerticalGaps.LastOrDefault().Right - rect.Left);
            if (distToLast < 0.01)
            {
                // If the current rectangle can be merged with the last one in the merged list,
                // replace the last rectangle with the merged rectangle
                var lastRect = mergedVerticalGaps.Last();
                mergedVerticalGaps[^1] = new PdfRectangle(lastRect.Left, lastRect.Bottom, rect.Right, rect.Top);
            }
            else
            {
                // If the current rectangle cannot be merged with the last one, add it to the merged list
                mergedVerticalGaps.Add(rect);
            }
        }

        return mergedVerticalGaps;
    }

    public List<PdfRectangle> GetHorizontalGaps()
    {
        GetRowsAndColumns();
        var horizontalGaps = GetGaps(Rows);
        var mergedHorizontalGaps = new List<PdfRectangle>();
        foreach (var rect in horizontalGaps)
        {
            var distToLast = Math.Abs(mergedHorizontalGaps.LastOrDefault().Bottom - rect.Top);
            if (distToLast < 0.01)
            {
                // If the current rectangle can be merged with the last one in the merged list,
                // replace the last rectangle with the merged rectangle
                var lastRect = mergedHorizontalGaps.Last();
                mergedHorizontalGaps[^1] = new PdfRectangle(rect.Left, rect.Bottom, lastRect.Right, lastRect.Top);
            }
            else
            {
                // If the current rectangle cannot be merged with the last one, add it to the merged list
                mergedHorizontalGaps.Add(rect);
            }
        }

        return mergedHorizontalGaps;
    }

    private List<PdfRectangle> GetGaps(List<IGrouping<double, Quadtree>> rowsOrColumns)
    {
        var gaps = new List<PdfRectangle>();
        foreach (var rowOrColumn in rowsOrColumns)
        {
            if (rowOrColumn.All(r => !r.IsFilled))
            {
                var x1 = rowOrColumn.Min(r => r.BoundingBox.Left);
                var y1 = rowOrColumn.Min(qt => qt.BoundingBox.Bottom);
                var bl = new PdfPoint(x1, y1);
                var x2 = rowOrColumn.Max(r => r.BoundingBox.Right);
                var y2 = rowOrColumn.Max(qt => qt.BoundingBox.Top);
                var tr = new PdfPoint(x2, y2);
                var gap = new PdfRectangle(bl, tr);
                gaps.Add(gap);
            }
        }

        return gaps;
    }

    private void GetRowsAndColumns()
    {
        if (Rows.Count == 0 && Columns.Count == 0)
        {
            // get nodes
            var nodes = GetDeepestLevelNodes();
            // get rows and columns
            Rows = nodes.GroupBy(n => n.BoundingBox.Centroid.Y).OrderByDescending(g => g.Key).ToList();
            Columns = nodes.GroupBy(n => n.BoundingBox.Centroid.X).OrderBy(g => g.Key).ToList();
        }
    }

    private void Split()
    {
        if (BoundingBox.Height - _resolution <= 0)
        {
            return;
        }

        var subWidth = (BoundingBox.Width / 2);
        var subHeight = (BoundingBox.Height / 2);
        var x = BoundingBox.Left;
        var y = BoundingBox.Bottom;

        var swRect = new PdfRectangle(x + subWidth, y, x + subWidth + subWidth, y + subHeight);
        var seRect = new PdfRectangle(x, y, x + subWidth, y + subHeight);
        var neRect = new PdfRectangle(x, y + subHeight, x + subWidth, y + subHeight + subHeight);
        var nwRect = new PdfRectangle(x + subWidth, y + subHeight, x + subWidth + subWidth, y + subHeight + subHeight);
        nw = new Quadtree(_level + 1, nwRect, _resolution, this);
        ne = new Quadtree(_level + 1, neRect, _resolution, this);
        sw = new Quadtree(_level + 1, swRect, _resolution, this);
        se = new Quadtree(_level + 1, seRect, _resolution, this);
    }

    private List<Quadtree> GetDeepestLevelNodes()
    {
        var nodes = new List<Quadtree>();
        if (IsDeepestLevel)
        {
            nodes.Add(this);
        }
        else
        {
            nodes.AddRange(nw!.GetDeepestLevelNodes());
            nodes.AddRange(ne!.GetDeepestLevelNodes());
            nodes.AddRange(sw!.GetDeepestLevelNodes());
            nodes.AddRange(se!.GetDeepestLevelNodes());
        }
        return nodes;
    }
}
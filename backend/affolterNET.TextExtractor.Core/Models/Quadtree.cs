using affolterNET.TextExtractor.Core.Extensions;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class Quadtree
{
    private static int _id = 0;
    
    private int _level;
    private List<PdfRectangle> _filled;
    private readonly double _resolution;
    private Quadtree? nw;
    private Quadtree? ne;
    private Quadtree? sw;
    private Quadtree? se;

    public Quadtree(PdfRectangle boundingBox, double resolution)
    {
        Id = _id;
        _level = 0;
        _filled = new List<PdfRectangle>();
        BoundingBox = boundingBox;
        _resolution = resolution;
        Split();
    }
    
    private Quadtree(int level, PdfRectangle boundingBox, double resolution, Quadtree parent)
    {
        _id++;
        Id = _id;
        _level = level;
        _filled = new List<PdfRectangle>();
        BoundingBox = boundingBox;
        _resolution = resolution;
        Parent = parent;
        Split();
    }

    public int Id { get; set; }
    private PdfRectangle BoundingBox { get; }
    public Quadtree? Parent { get; }
    public double DeepestLevelNodeWidth => DeepestLevelNodes.FirstOrDefault()?.BoundingBox.Width ?? -1;
    public double DeepestLevelNodeHeight => DeepestLevelNodes.FirstOrDefault()?.BoundingBox.Height ?? -1;
    private bool IsFilled { get; set; }
    private bool IsDeepestLevel => nw == null && ne == null && sw == null && se == null;
    private List<Quadtree> DeepestLevelNodes { get; } = new();
    
    public void Insert(PdfRectangle rect)
    {
        // if it does not overlap, return
        if (!rect.Overlaps(BoundingBox))
        {
            return;
        }
        
        // set to filled
        _filled.Add(rect);
        IsFilled = true;
        if (!IsDeepestLevel)
        {
            nw!.Insert(rect);
            ne!.Insert(rect);
            sw!.Insert(rect);
            se!.Insert(rect);
        }
    }

    public List<Quadtree> GetOverlappingNodes(PdfRectangle rect)
    {
        var list = GetNodes(rect).OrderBy(r => r.BoundingBox.Centroid.X).ToList();
        return list;
    }

    public BlockNodes GetRowsAndColumns(PdfRectangle blockBox)
    {
        // get rows and columns
        var nodes = GetOverlappingNodes(blockBox);
        var rows = nodes.GroupBy(n => n.BoundingBox.Centroid.Y).ToList();
        var columns = nodes.GroupBy(n => n.BoundingBox.Centroid.X).ToList();
        return new BlockNodes(rows, columns);
    }
    
    public List<PdfRectangle> GetHorizontalGaps(BlockNodes blockNodes)
    {
        // get gaps with the quadtree coordinate system
        var horizontalGaps = GetGaps(blockNodes.Rows);
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

        // enlarge gaps with the real "filled"-positions
        var fixedHorizontalGaps = new List<PdfRectangle>();
        foreach (var gap in mergedHorizontalGaps)
        {
            var closestToTheTop = gap.Top;
            var topNodes = _filled.Where(f => f.Bottom > gap.Top).ToList();
            if (topNodes.Count > 0)
            {
                closestToTheTop = topNodes.Min(f => f.Bottom);
            }

            var closestToTheBottom = gap.Bottom;
            var bottomNodes = _filled.Where(f => f.Top < gap.Bottom).ToList();
            if (bottomNodes.Count > 0)
            {
                closestToTheBottom = bottomNodes.Max(f => f.Top);
            }

            var bottomLeft = new PdfPoint(gap.Left, closestToTheBottom);
            var topRight = new PdfPoint(gap.Right, closestToTheTop);
            var fixedRect = new PdfRectangle(bottomLeft, topRight);
            fixedHorizontalGaps.Add(fixedRect);
        }

        return fixedHorizontalGaps;
    }

    public List<PdfRectangle> GetVerticalGaps(BlockNodes blockNodes)
    {
        // get gaps with the quadtree coordinate system
        var verticalGaps = GetGaps(blockNodes.Columns);
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

        // enlarge gaps with the real "filled"-positions
        var fixedVerticalGaps = new List<PdfRectangle>();
        foreach (var gap in mergedVerticalGaps)
        {
            var closestToTheLeft = gap.Left;
            var leftNodes = _filled.Where(f => f.Right < gap.Left).ToList();
            if (leftNodes.Count > 0)
            {
                closestToTheLeft = leftNodes.Max(f => f.Right);
            }
            
            var closestToTheRight = gap.Right;
            var rightNodes = _filled.Where(f => f.Left > gap.Right).ToList();
            if (rightNodes.Count > 0)
            {
                closestToTheRight = rightNodes.Min(f => f.Left);
            }
            
            var bottomLeft = new PdfPoint(closestToTheLeft, gap.Bottom);
            var topRight = new PdfPoint(closestToTheRight, gap.Top);
            var fixedRect = new PdfRectangle(bottomLeft, topRight);
            fixedVerticalGaps.Add(fixedRect);
        }

        return fixedVerticalGaps;
    }

    private List<Quadtree> GetNodes(PdfRectangle rect)
    {
        var list = new List<Quadtree>();
        // if it does not overlap, return
        if (!rect.Overlaps(BoundingBox))
        {
            return list;
        }
        if (!IsDeepestLevel)
        {
            list.AddRange(nw!.GetNodes(rect));
            list.AddRange(ne!.GetNodes(rect));
            list.AddRange(sw!.GetNodes(rect));
            list.AddRange(se!.GetNodes(rect));
        }
        else
        {
            list.Add(this);
        }

        return list;
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

    private void Split()
    {
        if (BoundingBox.Height - _resolution <= 0)
        {
            AddDeepestLevelNode(this);
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

    private void AddDeepestLevelNode(Quadtree node)
    {
        if (Parent == null)
        {
            DeepestLevelNodes.Add(node);
        }
        else
        {
            Parent.AddDeepestLevelNode(node);
        }
    }

    private class QuadTreeComparer : IComparer<Quadtree>
    {
        public int Compare(Quadtree? rect1, Quadtree? rect2)
        {
            if (rect1 == null || rect2 == null)
            {
                return 0;
            }

            if (rect1.BoundingBox.Centroid.X < rect2.BoundingBox.Centroid.X)
            {
                return -1;
            }

            if (rect1.BoundingBox.Centroid.X > rect2.BoundingBox.Centroid.X)
            {
                return 1;
            }

            if (rect1.BoundingBox.Centroid.Y < rect2.BoundingBox.Centroid.Y)
            {
                return -1;
            }

            if (rect1.BoundingBox.Centroid.Y > rect2.BoundingBox.Centroid.Y)
            {
                return 1;
            }

            return 0;
        }
    }
}
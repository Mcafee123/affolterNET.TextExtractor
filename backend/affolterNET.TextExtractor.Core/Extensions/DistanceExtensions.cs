using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class DistanceExtensions
{
    /// <summary>
    /// calculates the Euclidean distance between two points
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static double Distance(this PdfPoint a, PdfPoint b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static double SmallestInList(this IList<double> list, double range = 0.5)
    {
        if (list.Count == 0)
        {
            return -1;
        }

        var groups = list.FindCommonGroups(range, d => d);
        var avg = groups.Select(l => l.Select(tpl => tpl.Item1).Average()).ToList();
        avg.Sort();
        return avg.First();
    }

    public static double BiggestInList(this IList<double> list, double range = 0.5)
    {
        if (list.Count == 0)
        {
            return -1;
        }

        var groups = list.FindCommonGroups(range, d => d);
        var avg = groups.Select(l => l.Select(tpl => tpl.Item1).Average()).ToList();
        avg.Sort();
        return avg.Last();
    }

    public static double GetTopDistance(this ILineOnPage lowerLine, ILineOnPage? upperLine)
    {
        return upperLine?.BaseLineY - lowerLine.BaseLineY ?? LineOnPage.DefaultTopDistance;
    }

    public static bool Overlaps(this PdfRectangle first, PdfRectangle second, double tolerance = 0.01)
    {
        if (tolerance > 0)
        {
            var t = tolerance / 2;
            first = new PdfRectangle(new PdfPoint(first.BottomLeft.X - t, first.BottomLeft.Y - t),
                new PdfPoint(first.TopRight.X + t, first.TopRight.Y + t));
            second = new PdfRectangle(new PdfPoint(second.BottomLeft.X - t, second.BottomLeft.Y - t),
                new PdfPoint(second.TopRight.X + t, second.TopRight.Y + t));
        }

        var overlapsX = first.OverlapsX(second);
        var overlapsY = first.OverlapsY(second);
        return overlapsX && overlapsY;
    }

    public static bool OverlapsX(this PdfRectangle first, PdfRectangle second)
    {
        var aLeft = first.Left;
        var aRight = first.Right;
        var bLeft = second.Left;
        var bRight = second.Right;

        //      aLeft  aRight
        //      ---------
        //      |       |
        //      ---------
        //
        // bLeft  bRight
        // ---------
        // |       |
        // ---------
        // 
        // bLeft          bRight
        // ------------------
        // |                |
        // ------------------
        //
        if (aLeft < bRight && aLeft >= bLeft)
        {
            return true;
        }

        // aLeft  aRight
        // ---------
        // |       |
        // ---------
        //
        //      bLeft  bRight
        //      ---------
        //      |       |
        //      ---------
        // 
        if (aRight > bLeft && aRight <= bRight)
        {
            return true;
        }

        //
        // aLeft          aRight
        // ------------------
        // |                |
        // ------------------
        //
        //      bLeft  bRight
        //      ---------
        //      |       |
        //      ---------
        // 
        if (aLeft <= bLeft && aRight >= bRight)
        {
            return true;
        }

        return false;
    }


    public static bool OverlapsY(this PdfRectangle first, PdfRectangle second)
    {
        var aTop = first.Top;
        var aBottom = first.Bottom;
        var bTop = second.Top;
        var bBottom = second.Bottom;
        //                        --------- bTop            --------- bTop
        // aTop    ---------      |       |                 |       |
        //         |       |      --------- bBottom         |       |
        // aBottom ---------                                |       |
        //                                                  --------- bBottom
        if (aTop > bBottom && aTop <= bTop)
        {
            return true;
        }

        //                   
        // aTop    --------- 
        //         |       | --------- bTop
        // aBottom --------- |       |
        //                   --------- bBottom
        if (aBottom >= bBottom && aBottom < bTop)
        {
            return true;
        }

        // 
        // aTop    ---------
        //         |       |    --------- bTop
        //         |       |    |       |
        //         |       |    --------- bBottom
        // aBottom ---------
        if (aTop >= bTop && aBottom <= bBottom)
        {
            return true;
        }

        return false;
    }

    public static List<List<Tuple<double, T>>> FindCommonGroups<T>(this IList<T> values, double range,
        Func<T, double> doubleSelector)
    {
        var list = new List<List<Tuple<double, T>>>();
        if (values.Count == 0)
        {
            return list;
        }

        var sorted = values
            .OrderBy(doubleSelector)
            .Select(v => new Tuple<double, T>(doubleSelector(v), v))
            .ToList();

        var clusters = new List<List<Tuple<double, T>>>();
        var currentCluster = new List<Tuple<double, T>> { sorted[0] };

        for (var i = 1; i < sorted.Count; i++)
        {
            if (Math.Abs(sorted[i].Item1 - currentCluster.Last().Item1) <= range)
            {
                currentCluster.Add(sorted[i]);
            }
            else
            {
                clusters.Add(currentCluster);
                currentCluster = new List<Tuple<double, T>> { sorted[i] };
            }
        }

        clusters.Add(currentCluster);

        var mostCommonGroups = clusters
            .OrderByDescending(g => g.Count)
            .ToList();

        return mostCommonGroups;
    }

    public static bool MatchWithTolerance(this double currentPosition, double valueCompared, double tolerance)
    {
        return currentPosition - tolerance < valueCompared && currentPosition + tolerance > valueCompared;
    }

    public static IEnumerable<PdfRectangle> GetOverlapping(this List<PdfRectangle> rect)
    {
        return rect.Where(r1 => rect.Except(new[] { r1 }).Any(r2 => r1.OverlapsX(r2)));
    }

    public static List<PdfRectangle> GetIntersections(this List<PdfRectangle> rect1, List<PdfRectangle> rect2)
    {
        // make sure no overlapping rects in any list
        if (rect1.GetOverlapping().Any())
        {
            throw new InvalidOperationException("rectangles in rect1 are overlapping");
        }

        if (rect2.GetOverlapping().Any())
        {
            throw new InvalidOperationException("rectangles in rect2 are overlapping");
        }
        var overlappingGaps = new List<PdfRectangle>();
        foreach (var r1 in rect1)
        {
            var overlapping2List = rect2.Where(r2 => r1.OverlapsX(r2)).ToList();
            if (overlapping2List.Count == 0)
            {
                // r1 does not overlap anything in rect2
                continue;
            }

            // create new checkLine containing only overlap-rectangles
            foreach (var overlapping2 in overlapping2List)
            {
                // create new overlap-rectangle with the intersection of both
                var y = r1.Centroid.Y;
                var x1 = r1.Left >= overlapping2.Left ? r1.Left : overlapping2.Left;
                var x2 = r1.Right <= overlapping2.Right ? r1.Right : overlapping2.Right;
                var newRectangle = new PdfRectangle(x1, y, x2, y);
                overlappingGaps.Add(newRectangle);
            }
        }

        return overlappingGaps.OrderBy(g => g.Left).ToList();
    }
}
using affolterNET.TextExtractor.Core.Extensions;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Test.Extensions;

public class DistanceExtensionsTest
{
    [Theory]
    [InlineData(30.1, 30.01, 40.1, 40.01, 40.2, 40.02, 50.2, 50.02, false)]
    [InlineData(30.1, 30.01, 40.1, 40.01, 40, 40, 50.2, 50.02, true)]
    [InlineData(30.1, 30.01, 40.1, 40.01, 40.1, 40.01, 50.2, 50.02, false, 0)]
    [InlineData(30.1, 30.01, 40.1, 40.01, 40.1, 40.01, 50.2, 50.02, true, 0.1)]
    public void OverlapsTest(double firstX1, double firstY1, double firstX2, double firstY2, double secondX1, double secondY1, double secondX2, double secondY2, bool expectedResult, double tolerance = 0)
    {
        var first = Rect(firstX1, firstY1, firstX2, firstY2);
        var second = Rect(secondX1, secondY1, secondX2, secondY2);
        var result = first.Overlaps(second, tolerance);
        Assert.True(result == expectedResult);
    }

    [Theory]
    [InlineData(30, 50, 20, 40, true)]
    [InlineData(30, 50, 20, 60, true)]
    [InlineData(20, 40, 30, 50, true)]
    [InlineData(20, 60, 30, 50, true)]
    public void OverlapsXTest(double firstX1, double firstX2, double secondX1, double secondX2, bool expectedResult)
    {
        var first = Rect(firstX1, firstX2);
        var second = Rect(secondX1, secondX2);
        var result = first.OverlapsX(second);
        Assert.Equal(expectedResult, result);
    }
    
    [Fact]
    public void GetIntersectionsTest_1()
    {
        var list1 = Line(Rect(70, 95), Rect(94, 135));
        var list2 = Line();
        Assert.Throws<InvalidOperationException>(() => list1.GetIntersections(list2));
    }
    
    [Fact]
    public void GetIntersectionsTest_2()
    {
        var list1 = Line();
        var list2 = Line(Rect(94, 135), Rect(70, 95));
        Assert.Throws<InvalidOperationException>(() => list1.GetIntersections(list2));
    }
    
    [Fact]
    public void GetIntersectionsTest_3()
    {
        var list1 = Line(Rect(10, 50), Rect(70, 80));
        var list2 = Line(Rect(0, 99));
        var intersections = list1.GetIntersections(list2);
        Assert.Equal(2, intersections.Count);
        Assert.Equal(10, intersections.First().Left);
        Assert.Equal(50, intersections.First().Right);
    }
    
    [Fact]
    public void GetIntersectionsTest_4()
    {
        var list1 = Line(Rect(10, 50), Rect(70, 80), Rect(110, 150));
        var list2 = Line(Rect(0, 200));
        var intersections = list1.GetIntersections(list2);
        Assert.Equal(3, intersections.Count);
    }
    
    [Fact]
    public void GetIntersectionsTest_5()
    {
        var list1 = Line(Rect(0, 200));
        var list2 = Line(Rect(10, 50), Rect(70, 80), Rect(110, 150));
        var intersections = list1.GetIntersections(list2);
        Assert.Equal(3, intersections.Count);
    }
    
    private List<PdfRectangle> Line(params PdfRectangle[] rectangles)
    {
        var gaps = new List<PdfRectangle>();
        gaps.AddRange(rectangles);
        return gaps;
    }
    
    private PdfRectangle Rect(double x1, double x2)
    {
        return new PdfRectangle(x1, 0, x2, 0);
    }
    
    private PdfRectangle Rect(double bottomLeftX, double bottomLeftY, double topRightX, double topRightY)
    {
        return new PdfRectangle(bottomLeftX, bottomLeftY, topRightX, topRightY);
    }
}
using affolterNET.TextExtractor.Core.Helpers;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.Util;

namespace affolterNET.TextExtractor.Core.Services;

public class PdfWordExtractor: IWordExtractor
{
    private readonly IOutput _log;

    public PdfWordExtractor(IOutput log)
    {
        _log = log;
    }

    public IEnumerable<Word> GetWords(IReadOnlyList<Letter> letters)
    {
        var nnOptions = new NearestNeighbourWordExtractor.NearestNeighbourWordExtractorOptions();
        var nnWordExtractor = new NearestNeighbourWordExtractor(nnOptions);
        var words = nnWordExtractor.GetWords(letters).ToList();
        return words;
    }
}

/*
   /// <summary>
   /// Nearest neighbour word extractor options.
   /// </summary>
   public class NearestNeighbourWordExtractorOptions : IWordExtractorOptions
   {
       /// <summary>
       /// <inheritdoc/>
       /// Default value is -1.
       /// </summary>
       public int MaxDegreeOfParallelism { get; set; } = -1;
       
       /// <summary>
       /// The maximum distance between two letters (start and end base line points) within the same word, as a function of the two letters.
       /// <para>If the distance between the two letters is greater than this maximum, they will belong to different words.</para>
       /// <para>Default value is 20% of the Max(Width, PointSize) of both letters. If <see cref="TextOrientation"/> is Other, this distance is doubled.</para>
       /// </summary>
       public Func<Letter, Letter, double> MaximumDistance { get; set; } = (l1, l2) =>
       {
           double maxDist = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(
               Math.Abs(l1.GlyphRectangle.Width),
               Math.Abs(l2.GlyphRectangle.Width)),
               Math.Abs(l1.Width)),
               Math.Abs(l2.Width)),
               l1.PointSize), l2.PointSize) * 0.2;
           
           if (l1.TextOrientation == TextOrientation.Other || l2.TextOrientation == TextOrientation.Other)
           {
                return 2.0 * maxDist;
           }
           return maxDist;
       };
       
       /// <summary>
       /// The default distance measure used between two letters (start and end base line points).
       /// <para>Default value is the Euclidean distance.</para>
       /// </summary>
       public Func<PdfPoint, PdfPoint, double> DistanceMeasure { get; set; } = Distances.Euclidean;
       
       /// <summary>
       /// The distance measure used between two letters (start and end base line points) with axis aligned <see cref="TextOrientation"/>.
       /// <para>Only used if <see cref="GroupByOrientation"/> is set to <c>true</c>.</para>
       /// <para>Default value is the Manhattan distance.</para>
       /// </summary>
       public Func<PdfPoint, PdfPoint, double> DistanceMeasureAA { get; set; } = Distances.Manhattan;
       
       /// <summary>
       /// Function used to filter out connection between letters, e.g. check if the letters have the same color.
       /// If the function returns <c>false</c>, letters will belong to different words.
       /// <para>Default value checks whether the neighbour is a white space or not. If it is the case, it returns <c>false</c>.</para>
       /// </summary>
       public Func<Letter, Letter, bool> Filter { get; set; } = (_, l2) => !string.IsNullOrWhiteSpace(l2.Value);
       
       /// <summary>
       /// Function used prior searching for the nearest neighbour. If return false, no search will be done.
       /// <para>Default value checks whether the current letter is a white space or not. If it is the case, it returns false and no search is done.</para>
       /// </summary>
       public Func<Letter, bool> FilterPivot { get; set; } = l => !string.IsNullOrWhiteSpace(l.Value);
       
       /// <summary>
       /// If <c>true</c>, letters will be grouped by <see cref="TextOrientation"/> before processing.
       /// The <see cref="DistanceMeasureAA"/> will be used on axis aligned letters, and the <see cref="DistanceMeasure"/> on others.
       /// <para>If <c>false</c>, <see cref="DistanceMeasure"/> will be used for all letters, and <see cref="DistanceMeasureAA"/> won't be used.</para>
       /// <para>Default value is true.</para>
       /// </summary>
       public bool GroupByOrientation { get; set; } = true;
   }
*/
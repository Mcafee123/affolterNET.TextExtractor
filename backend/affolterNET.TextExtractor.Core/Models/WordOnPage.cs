using affolterNET.TextExtractor.Core.Extensions;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class WordOnPage: IWordOnPage
{
    private Word _word;
    private readonly double _baseLineGroupRange;

    public WordOnPage(int pageNr, Word word, double baseLineGroupRange)
    {
        PageNr = pageNr;
        _word = word;
        _baseLineGroupRange = baseLineGroupRange;
        var bottomLeft = new PdfPoint(word.Letters.Min(l => l.StartBaseLine.X), word.Letters.Min(l => l.StartBaseLine.Y));
        var topRight = new PdfPoint(word.Letters.Max(l => l.EndBaseLine.X),
            bottomLeft.Y + word.Letters.Max(l => l.PointSize));
        BoundingBox = new PdfRectangle(bottomLeft, topRight);
        BaseLineGroups = word.Letters.ToList().FindCommonGroups(_baseLineGroupRange, l => l.StartBaseLine.Y);
        BaseLineY = BaseLineGroups.First().First().Value;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Text, BoundingBox.BottomLeft.X, BoundingBox.BottomLeft.Y, BoundingBox.TopRight.X, BoundingBox.TopRight.Y);
    }

    public FoundGroups<Letter> BaseLineGroups { get; private set; }

    public WordOnPage Clone()
    {
        return new WordOnPage(PageNr, _word, _baseLineGroupRange);
    }

    public void ChangeWord(List<Letter> letters)
    {
        var newLetters = new List<Letter>();
        foreach (var letter in letters)
        {
            var newGlyphRectangle = new PdfRectangle(letter.GlyphRectangle.BottomLeft.X, letter.GlyphRectangle.BottomLeft.Y,
                letter.GlyphRectangle.BottomLeft.X + letter.Width, letter.GlyphRectangle.TopRight.Y);
            var l = new Letter(letter.Value, newGlyphRectangle, letter.StartBaseLine, letter.EndBaseLine,
                letter.Width, letter.FontSize, letter.Font, letter.RenderingMode, letter.StrokeColor, letter.FillColor,
                letter.PointSize, letter.TextSequence);
            newLetters.Add(l);
        }
        _word = new Word(newLetters);
        BoundingBox = _word.BoundingBox;
        BaseLineGroups = _word.Letters.ToList().FindCommonGroups(_baseLineGroupRange, l => l.StartBaseLine.Y);
        BaseLineY = BaseLineGroups.First().First().Value;
    }

    public LineOnPage? Line { get; set; }
    // public Footnote? Footnote { get; set; }
    public int PageNr { get; }

    /// <summary>
    /// The text of the word.
    /// </summary>
    public string Text => _word.Text;

    public bool HasText => !string.IsNullOrWhiteSpace(Text);

    /// <summary>
    /// The text orientation of the word.
    /// </summary>
    public TextOrientation TextOrientation => _word.TextOrientation;

    /// <summary>
    /// The rectangle completely containing the word.
    /// </summary>
    public PdfRectangle BoundingBox { get; private set; }

    /// <summary>
    /// The name of the font for the word.
    /// </summary>
    public string FontName => _word.FontName;

    public double FontSizeAvg {
        get
        {
            var fontSizes = _word.Letters.ToList()
                .FindCommonGroups<Letter>(0.1, l => l.PointSize).ToList();
            if (fontSizes.Count > 0)
            {
                return fontSizes.First().Average(tpl => tpl.Value);
            }

            throw new InvalidOperationException("Word with no letters found");
        }
    }

    /// <summary>
    /// The letters contained in the word.
    /// </summary>
    public IReadOnlyList<Letter> Letters => _word.Letters;

    public double BaseLineY { get; set; }
    public bool IsBelowY(double horizontalPoint)
    {
        return BoundingBox.Top < horizontalPoint;
    }

    public override string ToString()
    {
        return $"{Text} (x: {BoundingBox.Centroid.X}, y: {BoundingBox.Centroid.Y})";
    }
}

public interface IWordOnPage
{
    int PageNr { get; }
    string Text { get; }
    bool HasText { get; }
    TextOrientation TextOrientation { get; }
    string FontName { get; }
    double FontSizeAvg { get; }
    LineOnPage? Line { get; set; }
    PdfRectangle BoundingBox { get; }
    IReadOnlyList<Letter> Letters { get; }
    double BaseLineY { get; }
    bool IsBelowY(double horizontalPoint);
    WordOnPage Clone();
    void ChangeWord(List<Letter> letters);
    FoundGroups<Letter> BaseLineGroups { get; }
}
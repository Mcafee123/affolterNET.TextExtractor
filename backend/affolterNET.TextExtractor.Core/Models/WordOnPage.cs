using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class WordOnPage: IWordOnPage
{
    private Word _word;
    
    public WordOnPage(int pageNr, Word word)
    {
        PageNr = pageNr;
        _word = word;
        // check for GlyphRectangle-error
        // where top and bottom are the same
        // var lettersNoSpaces = word.Letters
        //     .Where(l => !string.IsNullOrWhiteSpace(l.Value))
        //     .ToList();
        // if (lettersNoSpaces.Count > 0 && lettersNoSpaces.All(l => Math.Abs(l.GlyphRectangle.Top - l.GlyphRectangle.Bottom) < 0.1))
        // {
        //     // make boundingbox 2/3 of the text-pointsize
        //     var boxHeight = word.Letters.Average(l => l.PointSize) / 3 * 2;
        //     var topRight = new PdfPoint(word.BoundingBox.Right, word.BoundingBox.Bottom + boxHeight);
        //     BoundingBox = new PdfRectangle(word.BoundingBox.BottomLeft, topRight);
        // }
        // else
        // {
        //     BoundingBox = word.BoundingBox;
        // }
        var bottomLeft = new PdfPoint(word.Letters.Min(l => l.StartBaseLine.X), word.Letters.Min(l => l.StartBaseLine.Y));
        var topRight = new PdfPoint(word.Letters.Max(l => l.EndBaseLine.X),
            bottomLeft.Y + word.Letters.Max(l => l.FontSize));
        BoundingBox = new PdfRectangle(bottomLeft, topRight);
    }

    public WordOnPage Clone()
    {
        return new WordOnPage(PageNr, _word);
    }

    public void ChangeWord(List<Letter> letters)
    {
        _word = new Word(letters);
        BoundingBox = _word.BoundingBox;
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
    
    /// <summary>
    /// The letters contained in the word.
    /// </summary>
    public IReadOnlyList<Letter> Letters => _word.Letters;

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
    LineOnPage? Line { get; set; }
    PdfRectangle BoundingBox { get; }
    IReadOnlyList<Letter> Letters { get; }
    bool IsBelowY(double horizontalPoint);
    WordOnPage Clone();
    void ChangeWord(List<Letter> letters);
}
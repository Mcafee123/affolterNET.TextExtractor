using affolterNET.TextExtractor.Core.Helpers;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class LetterJson
{
    public LetterJson()
    {
        
    }

    public LetterJson(Letter letter, IOutput log)
    {
        GlyphRectangle = letter.GlyphRectangle;
        StartBaseLine = letter.StartBaseLine;
        EndBaseLine = letter.EndBaseLine;
        Text = letter.Value;
        FontSize = letter.PointSize;
        IsBold = letter.Font.IsBold;
        IsItalic = letter.Font.IsItalic;
        Orientation = letter.TextOrientation.ToString();
    }

    public PdfPoint EndBaseLine { get; set; }
    public PdfPoint StartBaseLine { get; set; }
    public bool IsItalic { get; set; }
    public bool IsBold { get; set; }
    public string? Orientation { get; set; }
    public PdfRectangle GlyphRectangle { get; set; }
    public string Text { get; set; } = null!;
    public double FontSize { get; set; }
}
using affolterNET.TextExtractor.Core.Helpers;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class WordOnPageJson
{
    public WordOnPageJson()
    {
        
    }
    
    public WordOnPageJson(IWordOnPage word, IOutput log)
    {
        Id = word.Id;
        BoundingBox = word.BoundingBox;
        BaseLineY = word.BaseLineY;
        Text = word.Text;
        FontName = word.FontName;
        Orientation = word.TextOrientation.ToString();
        foreach (var letter in word.Letters)
        {
            Letters.Add(new LetterJson(letter, log));
        }
    }

    public int Id { get; set; }
    public double BaseLineY { get; set; }
    public PdfRectangle BoundingBox { get; set; }
    public string Text { get; set; } = null!;
    public int PageNr { get; set; }
    public string FontName { get; set; } = null!;
    public string? Orientation { get; set; }
    public List<LetterJson> Letters { get; set; } = new();
}
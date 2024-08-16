using affolterNET.TextExtractor.Core.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class PdfLineJson
{
    public PdfLineJson()
    {
        
    }
    
    public PdfLineJson(LineOnPage line, IOutput log)
    {
        foreach (var word in line)
        {
            Words.Add(new WordOnPageJson(word, log));
        }

        BoundingBox = line.BoundingBox;
        FontSizeAvg = line.FontSizeAvg;
        TopDistance = line.TopDistance;
        BaseLineY = line.BaseLineY;
    }

    public double BaseLineY { get; set; }
    public double TopDistance { get; set; }
    public double FontSizeAvg { get; set; }
    public PdfRectangle BoundingBox { get; set; }
    public List<WordOnPageJson> Words { get; set; } = new();
}
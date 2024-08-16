using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class PdfTextBlockJson
{
    public PdfTextBlockJson()
    {
        
    }

    public PdfTextBlockJson(IPdfTextBlock block, IOutput log)
    {
        Id = block.Id;
        BoundingBox = block.BoundingBox;
        foreach (var word in block.Words)
        {
            Words.Add(new WordOnPageJson(word, log));
        }

        foreach (var line in block.Lines)
        {
            Lines.Add(new PdfLineJson(line, log));
        }
    }

    public int Id { get; set; }
    public PdfRectangle BoundingBox { get; set; }
    public List<PdfLineJson> Lines { get; set; } = new();
    public List<WordOnPageJson> Words { get; set; } = new();
}
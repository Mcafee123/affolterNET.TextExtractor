using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class PdfPageJson: IJsonSaveable
{
    public PdfPageJson()
    {
        
    }

    public PdfPageJson(IPdfPage page, IOutput log)
    {
        Nr = page.Nr;
        PageNumberBlockId = page.PageNumberBlock?.Id;
        HeaderBlockIds = page.HeaderBlockIds;
        BoundingBox = page.BoundingBox;
        foreach (var block in page.Blocks.TextBlocks)
        {
            Blocks.Add(new PdfTextBlockJson(block, log));
        }
        foreach (var block in page.Blocks.ImageBlocks)
        {
            ImageBlocks.Add(new PdfImageBlockJson(block, log));
        }
    }

    public List<int> HeaderBlockIds { get; set; } = new();

    public int? PageNumberBlockId { get; set; }

    public List<PdfImageBlockJson> ImageBlocks { get; set; } = new();

    public int Nr { get; set; }
    public PdfRectangle BoundingBox { get; set; }
    public List<PdfTextBlockJson> Blocks { get; set; } = new();
}
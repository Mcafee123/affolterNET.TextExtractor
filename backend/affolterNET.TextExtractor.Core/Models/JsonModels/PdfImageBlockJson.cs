using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class PdfImageBlockJson
{
    public PdfImageBlockJson()
    {
        
    }

    public PdfImageBlockJson(IPdfImageBlock block, IOutput log)
    {
        BoundingBox = block.BoundingBox;
        Base64Image = block.Base64Image;
    }

    public string Base64Image { get; set; } = null!;

    public PdfRectangle BoundingBox { get; set; }
}
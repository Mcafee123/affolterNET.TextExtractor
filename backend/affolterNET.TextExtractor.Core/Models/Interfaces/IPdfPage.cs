using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfPage
{
    Page Page { get; }
    IPdfDoc Document { get; }
    PdfRectangle BoundingBox { get; }
    int Nr { get; }
    PdfBlocks Blocks { get; set; }
    bool VerifyBlocks(out string message);
}
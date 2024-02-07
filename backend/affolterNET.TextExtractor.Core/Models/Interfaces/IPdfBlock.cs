using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfBlock
{
    double TopDistance { get; set; }
    PdfRectangle BoundingBox { get; }
    IPdfPage Page { get; }
}
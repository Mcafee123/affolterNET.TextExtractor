using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfBlockBase
{
    double TopDistance { get; set; }
    PdfRectangle BoundingBox { get; }
    int PageNr { get; }
}
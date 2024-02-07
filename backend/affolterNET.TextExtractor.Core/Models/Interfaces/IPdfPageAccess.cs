using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfPageAccess : IPdfPage
{
    Page.Experimental ExperimentalAccess { get; }
    CropBox CropBox { get; }
}
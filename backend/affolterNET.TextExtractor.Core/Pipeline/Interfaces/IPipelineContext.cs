using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Pipeline.Interfaces;

public interface IPipelineContext: IDisposable
{
    string Filename { get; }
    double BigSpacesSize { get; }
    List<Word> OriginalWords { get; }
    IPdfDoc? Document { get; }
    void SetDocument(PdfDocument document);
    Stream? PdfStream { get; }
}
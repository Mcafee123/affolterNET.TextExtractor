using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Pipeline.Interfaces;

public interface IPipelineContext: IDisposable
{
    string Filename { get; }
    List<Word> OriginalWords { get; }
    IPdfDoc? Document { get; }
    void SetDocument(PdfDocument document);
    Stream? PdfStream { get; }
    List<IPdfImage> OriginalImages { get; }
    string? TextContent { get; set; }
    void AddSettings<T>(T settings) where T : class, IStepSettings, new();
    T GetSettings<T>() where T : class, IStepSettings, new();
}
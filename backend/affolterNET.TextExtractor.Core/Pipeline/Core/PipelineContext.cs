using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Pipeline.Core;

public class PipelineContext: IPipelineContext
{
    public PipelineContext(string filename)
    {
        Filename = filename;
    }

    public string Filename { get; }
    public List<Word> OriginalWords { get; } = new();
    public IPdfDoc? Document { get; private set; }

    public void SetDocument(PdfDocument document)
    {
        Document = new PdfDoc(Filename, document);
    }

    public override string ToString()
    {
        var pgCount = Document == null ? 0 : Document.Pages.Count;
        return $"{Filename}; Pages: {pgCount}";
    }

    public void Dispose()
    {
        Document?.Dispose();
    }
}
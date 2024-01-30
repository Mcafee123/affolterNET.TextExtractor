using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig;

namespace affolterNET.TextExtractor.Core.Pipeline.Core;

public class PipelineContext: AbstractPipelineContext
{
    public PipelineContext(string filename) : base(filename)
    {
    }

    public PipelineContext(Stream pdfStream, string filename) : base(pdfStream, filename)
    {
    }

    public override void SetDocument(PdfDocument document)
    {
        var doc = new PdfDoc(Filename, document);
        SetDocument(doc);
    }
}
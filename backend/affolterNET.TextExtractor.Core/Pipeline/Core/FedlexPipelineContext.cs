using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Core;

public class FedlexPipelineContext: PipelineContext, IFedlexPipelineContext
{
    public FedlexPipelineContext(string filename) : base(filename)
    {
    }

    public FedlexPipelineContext(Stream pdfStream, string filename) : base(pdfStream, filename)
    {
    }

    public double BigSpacesSize { get; } = 100;
}
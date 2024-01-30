namespace affolterNET.TextExtractor.Core.Pipeline.Interfaces;

public interface IFedlexPipelineContext: IPipelineContext
{
    // CleanWordsStep
    double BigSpacesSize { get; }
}
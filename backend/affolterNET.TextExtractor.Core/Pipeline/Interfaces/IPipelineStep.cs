namespace affolterNET.TextExtractor.Core.Pipeline.Interfaces;

public interface IPipelineStep
{
    void Execute(IPipelineContext context);
}
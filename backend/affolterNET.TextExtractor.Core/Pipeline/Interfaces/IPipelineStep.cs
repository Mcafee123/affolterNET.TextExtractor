namespace affolterNET.TextExtractor.Core.Pipeline.Interfaces;

public interface IPipelineStep
{
    Task Execute(IPipelineContext context);
}
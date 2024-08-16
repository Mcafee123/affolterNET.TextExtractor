namespace affolterNET.TextExtractor.Core.Pipeline.Interfaces;

public interface IBasicPdfPipeline
{
    void AddStep(IPipelineStep step);
    Task Execute(IPipelineContext context);
}
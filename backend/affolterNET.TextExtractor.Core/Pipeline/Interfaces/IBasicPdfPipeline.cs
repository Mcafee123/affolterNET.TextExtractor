namespace affolterNET.TextExtractor.Core.Pipeline.Interfaces;

public interface IBasicPdfPipeline
{
    Task Execute(IPipelineContext context);
}
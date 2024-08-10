using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Core;

public class ProcessingPipeline
{
    private readonly List<IPipelineStep> _steps;

    public ProcessingPipeline()
    {
        _steps = new List<IPipelineStep>();
    }

    public void AddStep(IPipelineStep step)
    {
        _steps.Add(step);
    }

    public async Task ExecutePipeline(IPipelineContext context)
    {
        foreach (var step in _steps)
        {
            await step.Execute(context);
        }
    } 
}

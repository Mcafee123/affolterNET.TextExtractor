using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class DetectFootnotesStep: IPipelineStep
{
    private readonly IOutput _log;

    public DetectFootnotesStep(IOutput log)
    {
        _log = log;
    }
    
    public class DetectFootnotesStepSettings: IStepSettings
    {

    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.None, "Detecting footnotes");
    }
}
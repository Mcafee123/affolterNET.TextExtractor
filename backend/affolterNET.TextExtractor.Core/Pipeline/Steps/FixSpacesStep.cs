using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class FixSpacesStep: IPipelineStep
{
    private readonly IFixSpacesService _fixSpacesService;
    private readonly IOutput _log;

    public FixSpacesStep(IFixSpacesService fixSpacesService, IOutput log)
    {
        _fixSpacesService = fixSpacesService;
        _log = log;
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Fixing space characters");
        var settings = context.GetSettings<DetectTextBlocksStep.DetectTextBlocksStepSettings>();
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }
        var missingSpacesFixed = _fixSpacesService.FixSpaces(context.Document, settings.BaseLineMatchingRange);
        _log.Write(EnumLogLevel.Info, $"Fixed {missingSpacesFixed} missing space characters");
    }
}
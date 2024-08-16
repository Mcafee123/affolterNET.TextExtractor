using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class DetectHeadersStep: IPipelineStep
{
    private readonly IHeaderService _headerService;
    private readonly IOutput _log;

    public DetectHeadersStep(IHeaderService headerService, IOutput log)
    {
        _headerService = headerService;
        _log = log;
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Detecting headers");
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }
        var headersDetected = _headerService.DetectHeaders(context.Document);
        _log.Write(EnumLogLevel.Info, $"Detected {headersDetected} headers");
    }
}
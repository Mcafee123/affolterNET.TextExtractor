using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class DetectPageNumberStep: IPipelineStep
{
    private readonly IPageNumberService _pageNumberService;
    private readonly IOutput _log;

    public DetectPageNumberStep(IPageNumberService pageNumberService, IOutput log)
    {
        _pageNumberService = pageNumberService;
        _log = log;
    }
    
    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Detecting page numbers");
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }
        var pageNumbersDetected = _pageNumberService.DetectPageNumberBlocks(context.Document);
        _log.Write(EnumLogLevel.Info, $"Detected {pageNumbersDetected} page numbers");
    }
}
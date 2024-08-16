using System.Net;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models.JsonModels;
using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace api.HttpTriggers;

public class UploadTrigger
{
    private readonly BasicPdfPipeline _pipeline;
    private readonly IOutput _log;
    private readonly ILogger _logger;

    public UploadTrigger(BasicPdfPipeline pipeline, ILogger<UploadTrigger> logger, IOutput log)
    {
        _pipeline = pipeline;
        _log = log;
        _logger = logger;
    }

    [Function(nameof(upload))]
    // ReSharper disable once InconsistentNaming
    public async Task<HttpResponseData> upload([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var traceEnabled = _logger.IsEnabled(LogLevel.Trace);
        _logger.LogTrace("Log Trace");
        _logger.LogDebug("Log Debug");
        _logger.LogInformation("Log Information");
        _logger.LogWarning("Log Warning");
        _logger.LogError("Log Error");
        _logger.LogCritical("Log Critical");
        _log.Write(EnumLogLevel.Trace, "Log Trace");
        _log.Write(EnumLogLevel.Debug, "Log Debug");
        _log.Write(EnumLogLevel.Info, "Log Information");
        _log.Write(EnumLogLevel.Warning, "Log Warning");
        _log.Write(EnumLogLevel.Error, "Log Error");
        _log.Write(EnumLogLevel.None, "Log Critical");
        var formData = await MultipartFormDataParser.ParseAsync(req.Body);
        var file = formData.Files.FirstOrDefault();
        if (file == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("upload did not contain a file");
            return badRequest;
        }

        var ms = new MemoryStream();
        await file.Data.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var pipelineContext = new PipelineContext(ms, file.FileName);
        // add custom settings this way:
        // =============================
        // pipelineContext.AddSettings(new CleanWordsStep.CleanWordsStepSettings() { BigSpacesSize = 120 });
        await _pipeline.Execute(pipelineContext);
        var pdfDocJson =  new PdfDocJson(pipelineContext.Document!, pipelineContext.TextContent, true, _log);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(pdfDocJson);
        return response;
    }
}
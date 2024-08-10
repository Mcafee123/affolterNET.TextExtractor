using System.Net;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace api.HttpTriggers;

public class BlobTrigger
{
    private readonly IExtractorFileService _extractorFileService;
    private readonly ILogger<BlobTrigger> _logger;
    private readonly IOutput _log;

    public BlobTrigger(IExtractorFileService extractorFileService, ILogger<BlobTrigger> logger, IOutput log)
    {
        _extractorFileService = extractorFileService;
        _logger = logger;
        _log = log;
    }

    [Function(nameof(listDocuments))]
    // ReSharper disable once InconsistentNaming
    public async Task<HttpResponseData> listDocuments([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var extracts = await _extractorFileService.ListDocuments();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(extracts);
        return response;
    }
    
    [Function(nameof(getDocument))]
    // ReSharper disable once InconsistentNaming
    public async Task<HttpResponseData> getDocument([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var folder = req.Query["folder"];
        if (string.IsNullOrWhiteSpace(folder))
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("folder query parameter is required");
            return badRequest;
        }
        
        var document = await _extractorFileService.GetDocument(folder);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(document);
        return response;
    }
    
    [Function(nameof(getPage))]
    // ReSharper disable once InconsistentNaming
    public async Task<HttpResponseData> getPage([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var folder = req.Query["folder"];
        if (string.IsNullOrWhiteSpace(folder))
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("\"folder\" query parameter is required");
            return badRequest;
        }

        var file = req.Query["file"];
        if (string.IsNullOrWhiteSpace(file))
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("\"file\" query parameter is required");
            return badRequest;
        }
        var document = await _extractorFileService.GetPage(folder, file);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(document);
        return response;
    }

    [Function(nameof(deleteDocument))]
    // ReSharper disable once InconsistentNaming
    public async Task<HttpResponseData> deleteDocument(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = nameof(deleteDocument) + "/{folder}")] HttpRequestData req,
        FunctionContext executionContext,
        string folder)
    {
        await _extractorFileService.DeleteDocument(folder);
        var response = req.CreateResponse(HttpStatusCode.OK);
        return response;
    }
}
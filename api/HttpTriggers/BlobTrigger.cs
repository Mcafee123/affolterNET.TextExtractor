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

    [Function(nameof(ListExtracts))]
    public async Task<HttpResponseData> ListExtracts([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var extracts = await _extractorFileService.ListDocuments();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(extracts);
        return response;
    }
    
    [Function(nameof(GetDocument))]
    public async Task<HttpResponseData> GetDocument([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
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
}
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
}
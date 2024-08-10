using System.Net;
using api.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace api.HttpTriggers;

public class InfoTrigger
{
    private readonly ILogger _logger;

    public InfoTrigger(ILogger<InfoTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(info))]
    // ReSharper disable once InconsistentNaming
    public async Task<HttpResponseData> info([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var traceEnabled = _logger.IsEnabled(LogLevel.Trace);
        var debugEnabled = _logger.IsEnabled(LogLevel.Debug);
        _logger.LogTrace($"Log Trace: {traceEnabled}");
        _logger.LogDebug($"Log Debug: {debugEnabled}");
        _logger.LogInformation("Log Information");
        _logger.LogWarning("Log Warning");
        _logger.LogError("Log Error");
        _logger.LogCritical("Log Critical");
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { Text = "Functions App is up and running!" });
        return response;
    }
}
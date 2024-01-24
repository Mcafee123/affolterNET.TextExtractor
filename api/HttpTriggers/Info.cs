using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace api.HttpTriggers;

public class Info
{
    private readonly ILogger _logger;

    public Info(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Info>();
    }

    [Function("Info")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        // response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        dynamic obj = new ExpandoObject();
        obj.text = "Welcome to Azure Functions!";
        await response.WriteAsJsonAsync(obj as object);

        return response;
        
    }
}
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
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
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
        FunctionContext executionContext)
    {
        dynamic obj = new ExpandoObject();
        obj.text = "Functions App is up and running!";
        return new JsonResult(obj);
    }
}
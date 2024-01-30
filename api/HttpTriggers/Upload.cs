using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace api.HttpTriggers;

public class Upload
{
    private readonly BasicPdfPipeline _pipeline;
    private readonly ILogger _logger;

    public Upload(BasicPdfPipeline pipeline, ILoggerFactory loggerFactory)
    {
        _pipeline = pipeline;
        _logger = loggerFactory.CreateLogger<Upload>();
    }

    [Function("Upload")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        FunctionContext executionContext)
    {
        var formData = await req.ReadFormAsync();
        var file = formData.Files.FirstOrDefault();
        if (file == null)
        {
            return new BadRequestObjectResult("upload did not contain a file");
        }

        var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var pipelineContext = new PipelineContext(ms, file.FileName);
        _pipeline.Execute(pipelineContext);
        var json = pipelineContext.Document!.Serialize();
        var result = new OkObjectResult(json); 
        return result;
    }
}
using Microsoft.AspNetCore.Mvc;

namespace affolterNET.TextExtractor.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly ILogger<InfoController> _logger;

    public InfoController(ILogger<InfoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Info endpoint called");

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        var isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        return Ok(new
        {
            status = "ok",
            service = "affolterNET.TextExtractor",
            version = "0.1.0",
            environment,
            runningInContainer = isContainer,
            timestamp = DateTime.UtcNow
        });
    }
}

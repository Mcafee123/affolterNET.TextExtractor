using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;

namespace affolterNET.TextExtractor.Web.Services;

public class LoggerOutput : IOutput
{
    private readonly ILogger<LoggerOutput> _logger;

    public LoggerOutput(ILogger<LoggerOutput> logger)
    {
        _logger = logger;
    }

    public void Write(EnumLogLevel logLevel, params string[] msg)
    {
        var message = string.Join("", msg);

        switch (logLevel)
        {
            case EnumLogLevel.Trace:
                _logger.LogTrace("{Message}", message);
                break;
            case EnumLogLevel.Debug:
                _logger.LogDebug("{Message}", message);
                break;
            case EnumLogLevel.Info:
                _logger.LogInformation("{Message}", message);
                break;
            case EnumLogLevel.Warning:
                _logger.LogWarning("{Message}", message);
                break;
            case EnumLogLevel.Error:
                _logger.LogError("{Message}", message);
                break;
            case EnumLogLevel.None:
            default:
                _logger.LogInformation("{Message}", message);
                break;
        }
    }

    public void WriteException(Exception ex)
    {
        _logger.LogError(ex, "Exception occurred in TextExtractor pipeline");
    }
}

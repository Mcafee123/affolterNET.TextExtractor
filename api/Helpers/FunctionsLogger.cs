using affolterNET.TextExtractor.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace api.Helpers;

public class FunctionsLogger: IOutput
{
    private readonly ILogger<FunctionsLogger> _logger;

    public FunctionsLogger(ILogger<FunctionsLogger> logger)
    {
        _logger = logger;
    }
    
    public void Write(EnumLogLevel logLevel, params string[] msg)
    {
        var txt = string.Join("", msg.Where(m => !(m.StartsWith("[") && m.EndsWith("]"))));
        switch (logLevel)
        {
            case EnumLogLevel.Trace:
                _logger.LogTrace(txt);
                return;
            case EnumLogLevel.Debug:
                _logger.LogDebug(txt);
                return;
            case EnumLogLevel.Info:
                _logger.LogInformation(txt);
                return;
            case EnumLogLevel.Warning:
                _logger.LogWarning(txt);
                return;
            case EnumLogLevel.Error:
                _logger.LogError(txt);
                return;
            case EnumLogLevel.None:
                _logger.LogCritical(txt);
                return;
        }
    }

    public void WriteException(Exception ex)
    {
        _logger.LogError(ex, "Processing-Pipeline Error");
    }
}
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Services;

namespace affolterNET.TextExtractor.Terminal.Services;

public class FileOutputter: IOutput
{
    private readonly LoggerContextProvider _loggerContextProvider;
    private readonly IOutput? _baseLogger;
    private readonly string _fileName;

    public FileOutputter(LoggerContextProvider loggerContextProvider, AnsiConsoleOutputter? baseLogger)
    {
        _loggerContextProvider = loggerContextProvider;
        _baseLogger = baseLogger;
        _fileName = _loggerContextProvider.CurrentContext;
    }
    
    public void Write(EnumLogLevel logLevel, params string[] msg)
    {
        _baseLogger?.Write(logLevel, msg);
        // log to list
        _loggerContextProvider.Write(_fileName, logLevel, msg);
    }

    public void WriteException(Exception ex)
    {
        _baseLogger?.WriteException(ex);
        // log to list
        _loggerContextProvider.WriteException(_fileName, ex);
    }
}
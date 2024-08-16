using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using Xunit.Abstractions;

namespace affolterNET.TextExtractor.Core.Test;

public class TestOutput: IOutput
{
    private readonly ITestOutputHelper _log;

    public TestOutput(ITestOutputHelper log)
    {
        _log = log;
    }

    public void Write(EnumLogLevel logLevel, params string[] msg)
    {
        var txt = string.Join("", msg.Where(m => !(m.StartsWith("[") && m.EndsWith("]"))));
        _log.WriteLine(txt);
    }

    public void WriteException(Exception ex)
    {
        _log.WriteLine(ex.GetBaseException().Message);
    }
}
using affolterNET.TextExtractor.Core.Helpers;

namespace affolterNET.TextExtractor.Core.Interfaces;

public interface IOutput
{
    void Write(EnumLogLevel logLevel, params string[] msg);
    void WriteException(Exception ex);
}
using affolterNET.TextExtractor.Core;
using affolterNET.TextExtractor.Core.Helpers;
using Spectre.Console;

namespace affolterNET.TextExtractor.Terminal;

public class AnsiConsoleOutputter: IOutput
{
    private readonly AnsiConsoleWrapper _wrp;

    public AnsiConsoleOutputter(AnsiConsoleWrapper wrp)
    {
        _wrp = wrp;
    }

    public void Write(EnumLogLevel logLevel, params string[] msg)
    {
        var hasMarkup = msg.Any(m => m.Contains("[/]", StringComparison.Ordinal));
        if (hasMarkup)
        {
            for (var i = 0; i<msg.Length; i++)
            {
                var m = msg[i];
                if (!m.StartsWith("[") || !m.EndsWith("]"))
                {
                    msg[i] = Markup.Escape(m);
                }
            }

            try
            {
                _wrp.MarkupLine(logLevel, string.Join("", msg));
            }
            catch (Exception)
            {
                _wrp.WriteLine(EnumLogLevel.Error, "tried to write:");
                _wrp.WriteLine(EnumLogLevel.Error, string.Join("", msg));
                throw;
            }
        }
        else
        {
            _wrp.WriteLine(logLevel, string.Join("", msg));
        }
    }

    public void WriteException(Exception ex)
    {
        AnsiConsole.WriteException(ex);
    }
}
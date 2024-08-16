using affolterNET.TextExtractor.Core.Helpers;

namespace affolterNET.TextExtractor.Core.Services;

public class LoggerContextProvider
{
    private readonly Dictionary<string, List<string>> _contexts = new();

    public LoggerContextProvider()
    {
        _contexts.Add("", new List<string>());
    }
    
    public void AddContext(string context)
    {
        _contexts.Add(context, new List<string>());
    }
    
    public string CurrentContext => _contexts.Last().Key;
    
    public List<string> this[string context] => _contexts.ContainsKey(context) ? _contexts[context] : [];

    public void Write(string context, EnumLogLevel logLevel, params string[] msg)
    {
        _contexts[context].Add($"{logLevel}: {string.Join(" ", msg)}");
    }

    public void WriteException(string context, Exception ex)
    {
        _contexts[context].Add($"{EnumLogLevel.Error}: {ex.Message}");
        _contexts[context].Add($"{EnumLogLevel.Error}: {ex.GetBaseException().Message}");
    }
}
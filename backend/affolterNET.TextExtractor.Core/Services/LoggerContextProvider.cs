using affolterNET.TextExtractor.Core.Helpers;

namespace affolterNET.TextExtractor.Core.Services;

public class LoggerContextProvider
{
    private readonly Dictionary<string, List<string>> _contexts = new();
    private string _currentContext = "";
    
    public LoggerContextProvider()
    {
        _contexts.Add(_currentContext, new List<string>());
    }
    
    public void AddContext(string context)
    {
        lock (_currentContext)
        {
            _contexts.Add(context, new List<string>());
            _currentContext = context;
        }
    }
    
    public string CurrentContext => _currentContext;
    
    public List<string> this[string context] => _contexts.ContainsKey(context) ? _contexts[context] : [];

    public void Write(string context, EnumLogLevel logLevel, params string[] msg)
    {
        lock (_currentContext)
        {
            _contexts[context].Add($"{logLevel}: {string.Join(" ", msg)}");
        }
    }

    public void WriteException(string context, Exception ex)
    {
        lock (_currentContext)
        {
            _contexts[context].Add($"{EnumLogLevel.Error}: {ex.Message}");
            _contexts[context].Add($"{EnumLogLevel.Error}: {ex.GetBaseException().Message}");
        }
    }

    public void ResetContext()
    {
        lock (_currentContext)
        {
            _currentContext = "";
        }
    }
}
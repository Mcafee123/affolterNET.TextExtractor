using Microsoft.Extensions.Configuration;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class ConfigurationExtensions
{
    public static string GetAndCheckValue(this IConfiguration cfg, string key)
    {
        var val = cfg.GetValue<string>(key);
        if (string.IsNullOrWhiteSpace(val))
        {
            throw new Exception($"Configuration Value \"{key}\" not available");
        }

        return val;
    }
}
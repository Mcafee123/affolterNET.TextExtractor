using System.Text.RegularExpressions;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class ErrorsExtensions
{
    public static List<string> SplitErrors(this string errors)
    {
        var split = errors
            .Split("\n")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        return split;
    }

    public static List<string> SplitErrors(this string errors, Regex? rx, string? group = null)
    {
        var split = errors.SplitErrors();
        if (rx == null)
        {
            return split;
        }

        var matches = split.Select(l => rx.Match(l)).Where(ma => ma.Success).ToList();
        if (group == null)
        {
            split = matches.Select(ma => ma.Value).ToList();
            return split;
        }

        split = matches
            .Where(ma => ma.Groups.ContainsKey(group))
            .Select(ma => ma.Groups[group].Value)
            .ToList();
        return split;
    }
}
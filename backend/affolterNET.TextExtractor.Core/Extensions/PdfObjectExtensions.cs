using System.Text.RegularExpressions;
using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Extensions;

public static partial class PdfObjectExtensions
{
    public static bool IsFraction(this IWordOnPage word, ILineOnPage line)
    {
        var wordIdx = line.IndexOf(word);
        if (line.Count <= wordIdx + 2)
        {
            return false;
        }
        var slash = line[wordIdx + 1].Text == "/";
        if (!slash)
        {
            return false;
        }

        var rx = SingleNumbRegex();
        var number = rx.Match(line[wordIdx + 2].Text).Success;
        return number;
    }
    
    [GeneratedRegex("^[\\d]$")]
    private static partial Regex SingleNumbRegex();
}
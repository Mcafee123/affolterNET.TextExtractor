namespace affolterNET.TextExtractor.Core.Extensions;

public static class StringExtensions
{
    public static bool IsNumeric(this string str)
    {
        foreach (var c in str.Trim())
        {
            if (c is < '0' or > '9')
            {
                return false;   
            }
        }

        return true;
    }
}
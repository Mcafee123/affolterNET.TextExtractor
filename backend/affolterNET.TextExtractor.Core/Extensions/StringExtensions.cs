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
    
    public static bool IsStar(this string str)
    {
        if (str.Length == 1 && str[0] == '*')
        {
            return true;
        }

        return false;
    }
    
    public static bool IsNumericOrStar(this string str)
    {
        if (str.IsStar())
        {
            return true;
        }

        return str.IsNumeric();
    }
}
using System.Globalization;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class DateTimeStringConverter
{
    public static string ToFolderName(this DateTime dateTime)
    {
        return
            $"{dateTime.Year:0000}_{dateTime.Month:00}_{dateTime.Day:00}__{dateTime.Hour:00}_{dateTime.Minute:00}_{dateTime.Second:00}";
    }
    
    public static bool FromFolderName(this string folderName, out DateTime dt)
    {
        dt = DateTime.MinValue;
        if (folderName.Length < 20)
        {
            return false;
        }

        if (DateTime.TryParseExact($"{folderName.Substring(0, 20)}", "yyyy_MM_dd__HH_mm_ss", null, DateTimeStyles.None,
            out var created))
        {
            dt = created;
            return true;
        }

        return false;
    }
}

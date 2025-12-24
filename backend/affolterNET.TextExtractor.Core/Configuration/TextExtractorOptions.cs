using affolterNET.Web.Core.Models;
using affolterNET.Web.Core.Options;

namespace affolterNET.TextExtractor.Core.Configuration;

public class TextExtractorOptions: IConfigurableOptions<TextExtractorOptions>
{
    public static string SectionName => "affolterNET:TextExtractor:TextExtractorSettings";
    public static TextExtractorOptions CreateDefaults(AppSettings settings)
    {
        return new TextExtractorOptions(settings);
    }

    public void CopyTo(TextExtractorOptions options)
    {
        options.AppUrl = AppUrl;
    }

    public TextExtractorOptions(): this(new AppSettings())
    {
        
    }
    
    private TextExtractorOptions(AppSettings settings)
    {
        AppUrl = "";
    }

    public string AppUrl { get; set; }
}
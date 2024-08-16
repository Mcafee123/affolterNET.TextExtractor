using affolterNET.TextExtractor.Core.Interfaces;

namespace affolterNET.TextExtractor.Core.Models.JsonModels;

public class PdfDocJson: IJsonSaveable
{
    public PdfDocJson()
    {
        
    }

    public PdfDocJson(IPdfDoc pdfDoc, string? textContent, bool serializePages, IOutput log)
    {
        Filename = pdfDoc.Filename;
        FontNames = string.Join(", ", pdfDoc.FontSizes?.AllFontNames ?? new List<string>());
        FontGroups = pdfDoc.FontSizes?
            .Select(fs => $"{fs.GroupId + 1}: {Math.Round(fs.AvgFontSize, 2):##.00} (Words: {fs.WordCount}, Min: {Math.Round(fs.MinFontSize, 2):##.00}, Max: {Math.Round(fs.MaxFontSize, 2):##.00})")
            .ToList() ?? new List<string>();
        TextContent = textContent;

        if (serializePages)
        {
            foreach (var page in pdfDoc.Pages)
            {
                Pages.Add(new PdfPageJson(page, log));
            }
        }
        
        foreach (var footnote in pdfDoc.Footnotes)
        {
            Footnotes.Add(new PdfFootnoteJson(footnote, log));
        }
    }

    public string? TextContent { get; set; }

    public List<string> FontGroups { get; set; } = new();
    public string Filename { get; set; } = null!;
    public string FontNames { get; set; } = null!;
    public List<PdfPageJson> Pages { get; set; } = new();
    public List<string> PageNames { get; set; } = new();
    public List<PdfFootnoteJson> Footnotes { get; set; } = new();
}
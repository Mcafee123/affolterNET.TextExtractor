using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class AnalyzeLineSpacingStep: IPipelineStep
{
    private readonly IOutput _log;

    public AnalyzeLineSpacingStep(IOutput log)
    {
        _log = log;
    }
    
    public void Execute(IPipelineContext context)
    {
        context.Document!.FontSizes = new FontSizeSettings(context.Document.Words);
        var lineGroups = GetLineGroupsByFontSize(context.Document);
        foreach (var groupId in lineGroups.Keys)
        {
            var group = lineGroups[groupId];
            var fontSizeSetting = context.Document.FontSizes[groupId];
            foreach (var line in group)
            {
                var topLine = group.FindLineOnTop(line);
                var topDistance = line.GetTopDistance(topLine);
                fontSizeSetting.AddSpacing(topDistance);
            }
        }
    }

    private Dictionary<int, PdfLines> GetLineGroupsByFontSize(IPdfDoc doc)
    {
        var lineGroups = new Dictionary<int, PdfLines>();
        foreach (var fsSettings in doc.FontSizes)
        {
            lineGroups.Add(fsSettings.GroupId, new PdfLines());
        }

        foreach (var page in doc.Pages)
        {
            foreach (var line in page.Lines)
            {
                var group = doc.FontSizes.GetGroup(line.FontSizeAvg);
                lineGroups[group.GroupId].Add(line);
            }
        }

        return lineGroups;
    }
}
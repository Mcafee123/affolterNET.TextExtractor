using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
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
        _log.Write(EnumLogLevel.Info, "Analyzing spacing between lines");
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }

        var words = context.Document.Pages
            .SelectMany(p => p.Blocks.TextBlocks
                .SelectMany(b => b.Lines
                    .SelectMany(l => l))).ToList();
        context.Document.FontSizes = new FontSizeSettings(words);
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
        foreach (var fsSettings in doc.FontSizes!)
        {
            lineGroups.Add(fsSettings.GroupId, new PdfLines());
        }

        foreach (var page in doc.Pages)
        {
            // foreach (var line in page.Lines)
            foreach (var line in page.Blocks.TextBlocks.SelectMany(b => b.Lines))
            {
                var group = doc.FontSizes.GetGroup(line.FontSizeAvg);
                lineGroups[group.GroupId].Add(line);
            }
        }

        return lineGroups;
    }
}
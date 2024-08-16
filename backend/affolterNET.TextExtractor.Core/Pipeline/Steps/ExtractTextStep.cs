using System.Text;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class ExtractTextStep: IPipelineStep
{
    private readonly IOutput _log;

    public ExtractTextStep(IOutput log)
    {
        _log = log;
    }
    
    public class ExtractTextStepSettings: IStepSettings
    {
        public bool ExtractText { get; set; } = true;
    }
    
    public void Execute(IPipelineContext context)
    {
        var settings = context.GetSettings<ExtractTextStepSettings>();
        if (settings.ExtractText)
        {
            _log.Write(EnumLogLevel.Info, "Extracting text");
            if (context.Document == null)
            {
                throw new NullReferenceException(
                    $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
            }

            var exclude = (IWordOnPage word) => context.Document.Footnotes.Any(fn => fn.Contains(word));
            var sb = new StringBuilder();
            foreach (var page in context.Document.Pages)
            {
                foreach (var block in page.Blocks.TextBlocks)
                {
                    sb.Append(block.GetText(exclude));
                    sb.Append("\n\n");
                }

                sb.Append("\n");
            }

            context.TextContent = sb.ToString();
        }
    }
}
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models.JsonModels;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class SerializeToJsonStep : IPipelineStep
{
    private readonly string _outputFolder;
    private readonly IOutput _log;

    public SerializeToJsonStep(string outputFolder, IOutput log)
    {
        _outputFolder = outputFolder;
        _log = log;
    }

    public void Execute(IPipelineContext context)
    {
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }
        var prefix = Path.GetFileNameWithoutExtension(context.Document.Filename);
        var toSerialize = new PdfDocJson(context.Document, null, false, _log);
        foreach (var page in context.Document.Pages)
        {
            var filename = $"{prefix}__page_{page.Nr}.json";
            toSerialize.PageNames.Add(filename);
            var pageJsonPath = Path.Combine(_outputFolder, filename);
            if (File.Exists(pageJsonPath))
            {
                File.Delete(pageJsonPath);
            }

            page.SerializePdfPage(pageJsonPath, _log);
        }
        
        var documentJsonPath = Path.Combine(_outputFolder, $"{prefix}__doc.json");
        if (File.Exists(documentJsonPath))
        {
            File.Delete(documentJsonPath);
        }
        toSerialize.SerializeAndSave(documentJsonPath);
    }
}
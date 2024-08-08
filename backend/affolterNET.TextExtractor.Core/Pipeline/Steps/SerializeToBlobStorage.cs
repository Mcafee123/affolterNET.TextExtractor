using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models.JsonModels;
using affolterNET.TextExtractor.Core.Models.StorageModels;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class SerializeToBlobStorage : IPipelineStep
{
    private readonly IExtractorFileService _extractorFileService;
    private readonly IOutput _log;

    public SerializeToBlobStorage(IExtractorFileService extractorFileService, IOutput log)
    {
        _extractorFileService = extractorFileService;
        _log = log;
    }

    public void Execute(IPipelineContext context)
    {
        _log.Write(EnumLogLevel.Info, "Serializing to blob storage");
        if (context.Document == null)
        {
            throw new NullReferenceException(
                $"context.Document not initialized. Run {nameof(ReadPagesStep)} before this step");
        }
        var prefix = Path.GetFileNameWithoutExtension(context.Document.Filename);
        var documentJsonName = Path.Combine($"{prefix}.json");
        var doc = new Document(documentJsonName);
        var toSerialize = new PdfDocJson(context.Document, null, false, _log);
        foreach (var page in context.Document.Pages)
        {
            var filename = $"{prefix}__page_{page.Nr}.json";
            toSerialize.PageNames.Add(filename);
            var pdfPage = new PdfPageJson(page, _log);
            var pageJsonStream = pdfPage.Serialize();
            doc.Pages.Add(new Page(filename, pageJsonStream));
        }

        var docJsonStream = toSerialize.Serialize();
        doc.Content = docJsonStream;
        _extractorFileService.UploadDocument(doc).GetAwaiter().GetResult();
        _log.Write(EnumLogLevel.Info, $"Documents uploaded: {doc.Pages.Count + 1}");
    }
}
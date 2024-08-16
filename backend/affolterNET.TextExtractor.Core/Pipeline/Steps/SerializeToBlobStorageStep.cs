using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models.JsonModels;
using affolterNET.TextExtractor.Core.Models.StorageModels;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Services;

namespace affolterNET.TextExtractor.Core.Pipeline.Steps;

public class SerializeToBlobStorageStep : IPipelineStep
{
    private readonly IExtractorFileService _extractorFileService;
    private readonly LoggerContextProvider _loggerContextProvider;
    private readonly IOutput _log;

    public SerializeToBlobStorageStep(IExtractorFileService extractorFileService, LoggerContextProvider loggerContextProvider, IOutput log)
    {
        _extractorFileService = extractorFileService;
        _loggerContextProvider = loggerContextProvider;
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
        
        // collect logs
        var logName = Path.Combine($"{prefix}__log.txt");
        var logPage = WriteLogFile(context.Filename, logName);
        doc.Pages.Add(logPage);
        
        // create doc
        var docJsonStream = toSerialize.Serialize();
        doc.Content = docJsonStream;
        _extractorFileService.UploadDocument(doc).GetAwaiter().GetResult();
        _log.Write(EnumLogLevel.Info, $"Documents uploaded: {doc.Pages.Count + 1}");
    }

    private Page WriteLogFile(string fileName, string logName)
    {
        // create text stream
        var ms = new MemoryStream(); 
        var sr = new StreamWriter(ms);
        var logList = _loggerContextProvider[fileName];
        foreach (var log in logList)
        {
            sr.WriteLine(log);
        }

        ms.Seek(0, SeekOrigin.Begin);
        var logPage = new Page(logName, ms);
        return logPage;
    }
}
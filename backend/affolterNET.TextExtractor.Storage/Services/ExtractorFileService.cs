using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models.JsonModels;
using affolterNET.TextExtractor.Core.Models.StorageModels;

namespace affolterNET.TextExtractor.Storage.Services;

public class ExtractorFileService : IExtractorFileService
{
    private const string ContainerName = "extractor";
    private readonly BlobStorageService _blobStorageService;
    private readonly IOutput _log;

    public ExtractorFileService(BlobStorageService blobStorageService, IOutput log)
    {
        _blobStorageService = blobStorageService;
        _log = log;
    }

    public async Task<List<Document>> ListDocuments()
    {
        var blobs = await _blobStorageService.GetFolders(ContainerName);
        var docs = new List<Document>();
        foreach (var b in blobs)
        {
            if (b.IsPrefix)
            {
                // created
                if (!b.Prefix.FromFolderName(out var created) || b.Prefix.Length < 23)
                {
                    continue;
                }

                // name
                var name = b.Prefix.Substring(22);
                if (name.EndsWith("/"))
                {
                    name = name.Substring(0, name.Length - 1);
                }

                var doc = new Document(name, created);
                docs.Add(doc);
            }
        }

        return docs;
    }

    public async Task<PdfDocJson> GetDocument(string folder)
    {
        var docName = folder.Substring(22);
        var docFullName = $"{folder}/{docName}.json";
        var doc = await _blobStorageService.GetStringBlob(ContainerName, docFullName);
        var pdfDoc = doc.DeserializeFromString<PdfDocJson>();
        var page1FullName = $"{folder}/{docName}__page_1.json";
        var page1 = await _blobStorageService.GetStringBlob(ContainerName, page1FullName);
        var pdfPage1 = page1.DeserializeFromString<PdfPageJson>();
        pdfDoc.Pages.Add(pdfPage1);
        return pdfDoc;
    }

    public async Task<PdfPageJson> GetPage(string folder, string fileNumber)
    {
        var docName = folder.Substring(22);
        var pageFullName = $"{folder}/{docName}__page_{fileNumber}.json";
        var page = await _blobStorageService.GetStringBlob(ContainerName, pageFullName);
        return page.DeserializeFromString<PdfPageJson>();
    }

    public async Task UploadDocument(Document doc)
    {
        if (doc.Content == null)
        {
            throw new InvalidOperationException($"{doc.Filename}: document content is null");
        }

        var folder = doc.Foldername;
        var path = $"{folder}/{doc.Filename}";
        await _blobStorageService.UploadFileAsync(ContainerName, path, doc.Content);
        _log.Write(EnumLogLevel.Debug, $"document uploaded: {doc.Filename}");
        foreach (var page in doc.Pages)
        {
            var pagePath = $"{folder}/{page.Filename}";
            if (page.Content == null)
            {
                throw new InvalidOperationException($"{page.Filename}: page content is null");
            }

            await _blobStorageService.UploadFileAsync(ContainerName, pagePath, page.Content);
            _log.Write(EnumLogLevel.Debug, $"document uploaded: {page.Filename}");
        }
    }

    // public async Task DeleteDocument(string dateFolderName)
    // {
    //     await _blobStorageService.DeleteByHierarchy(ContainerName, dateFolderName);
    // }
}
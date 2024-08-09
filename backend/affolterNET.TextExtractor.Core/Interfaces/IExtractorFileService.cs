using affolterNET.TextExtractor.Core.Models.JsonModels;
using affolterNET.TextExtractor.Core.Models.StorageModels;

namespace affolterNET.TextExtractor.Core.Interfaces;

public interface IExtractorFileService
{
    Task UploadDocument(Document doc);
    Task<List<Document>> ListDocuments();
    Task<PdfDocJson> GetDocument(string folder);
    Task<PdfPageJson> GetPage(string folder, string fileNumber);
}
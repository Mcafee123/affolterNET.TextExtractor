using affolterNET.TextExtractor.Core.Models.StorageModels;

namespace affolterNET.TextExtractor.Core.Interfaces;

public interface IExtractorFileService
{
    Task UploadDocument(Document doc);
    Task<List<Document>> ListDocuments();
}
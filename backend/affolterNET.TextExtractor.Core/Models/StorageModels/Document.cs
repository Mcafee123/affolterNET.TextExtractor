using affolterNET.TextExtractor.Core.Extensions;

namespace affolterNET.TextExtractor.Core.Models.StorageModels;

public class Document: DocumentBase
{
    public Document(string fileName): base(fileName)
    {
        Created = DateTime.UtcNow;
    }
    
    public Document(string fileName, DateTime created): base(fileName)
    {
        Created = created;
    }
    
    public DateTime Created { get; }
    public List<Page> Pages { get; set; } = new();

    public string Foldername => $"{Created.ToFolderName()}__{Path.GetFileNameWithoutExtension(Filename)}";
}
namespace affolterNET.TextExtractor.Core.Models.StorageModels;

public class DocumentBase
{
    public DocumentBase(string filename)
    {
        Filename = filename;
    }
    
    public string Filename { get; set; }
    public Stream? Content { get; set; }
}
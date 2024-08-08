namespace affolterNET.TextExtractor.Core.Models.StorageModels;

public class Page: DocumentBase
{
    public Page(string filename, Stream content): base(filename)
    {
        Content = content;
    }
}
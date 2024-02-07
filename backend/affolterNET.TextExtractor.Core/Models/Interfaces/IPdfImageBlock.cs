namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfImageBlock: IPdfBlock
{
    string Base64Image { get; }
}
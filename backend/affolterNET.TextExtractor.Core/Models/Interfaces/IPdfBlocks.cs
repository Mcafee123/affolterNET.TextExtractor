namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfBlocks : IList<IPdfBlock>
{
    List<IPdfTextBlock> TextBlocks { get; }
    List<IPdfImageBlock> ImageBlocks { get; }
    void AddRange(List<IPdfBlock> items);
}
namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfBlocks : IList<IPdfBlock>
{
    List<IPdfTextBlock> TextBlocks { get; }
    List<IPdfImageBlock> ImageBlocks { get; }
    void AddRange(IEnumerable<IPdfBlock> items);
    void AddRange(IEnumerable<IPdfBlockBase> items);
}
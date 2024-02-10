using affolterNET.TextExtractor.Core.Models.Interfaces;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfBlocks: BlocksBase<IPdfBlock, IPdfTextBlock, IPdfImageBlock>, IPdfBlocks
{
    public PdfBlocks()
    {
        
    }

    public PdfBlocks(IPdfBlock block) : base(block)
    {
        
    }
}
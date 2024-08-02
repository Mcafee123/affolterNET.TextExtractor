using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Services;

public class HeaderService: IHeaderService
{
    private readonly IOutput _log;

    public HeaderService(IOutput log)
    {
        _log = log;
    }
    
    public int DetectHeaders(IPdfDoc doc)
    {
        var allBlocks = doc.Pages.SelectMany(p => p.Blocks.TextBlocks).ToList();
        var sameBlocks = new Dictionary<IPdfTextBlock, List<IPdfTextBlock>>();
        foreach (var block in allBlocks)
        {
            var addedIds = sameBlocks.SelectMany(kvp => kvp.Value.Select(b => b.Id));
            if (addedIds.Contains(block.Id))
            {
                continue;
            }

            var same = allBlocks.Where(b => b.Id != block.Id && b.IsSameBlock(block)).ToList();
            if (same.Count > doc.Pages.Count / 3) // if the same block is on more than 1/3 of the pages, it is a header
            {
                same.Add(block);
                sameBlocks.Add(block, same);
            }
        }

        foreach (var page in doc.Pages)
        {
            var headerBlocks = sameBlocks.SelectMany(kvp => kvp.Value).Select(b => b.Id);
            page.HeaderBlockIds.AddRange(headerBlocks);
        }

        return sameBlocks.Keys.Count;
    }
}
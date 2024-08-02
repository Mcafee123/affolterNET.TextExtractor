using System.Text.RegularExpressions;
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Services.Interfaces;

namespace affolterNET.TextExtractor.Core.Services;

public class PageNumberService: IPageNumberService
{
    private readonly IOutput _log;
    private readonly double _pgNumMaxY;
    private readonly Regex _pageNrAndTotalRx;

    public PageNumberService(IOutput log, double pgNumMaxY = 45)
    {
        _log = log;
        _pgNumMaxY = pgNumMaxY;
        _pageNrAndTotalRx = new Regex(@"[\d]+\s*\/\s*[\d]+");
    }
    
    public int DetectPageNumberBlocks(IPdfDoc doc)
    {
        var pgNrCount = 0;
        foreach (var page in doc.Pages)
        {
            var footer = page.Blocks.TextBlocks.LastOrDefault(IsPageNumber);
            if (footer != null)
            {
                pgNrCount++;
                AddPageNumber(footer, page);
            }
        }

        return pgNrCount;
    }

    private bool IsPageNumber(IPdfTextBlock tb)
    {
        var lastLine = tb.Lines.LastOrDefault();
        var lastWord = lastLine?.LastOrDefault(w => w.HasText);
        if (lastWord == null)
        {
            throw new InvalidOperationException($"no lines or no words on last line found on page {tb.Page?.Nr}");
        }
            
        if (lastLine!.BoundingBox.Top > _pgNumMaxY)
        {
            return false;
        }
        
        // page number and page count?
        var ma = _pageNrAndTotalRx.Match(lastLine.ToString());
        if (ma.Success)
        {
            return true;
        }

        // simple number?
        return lastWord.Text.IsNumeric();
    }

    private void AddPageNumber(IPdfTextBlock block, IPdfPage page)
    {
        page.SetPageNumberBlock(block);
    }
}
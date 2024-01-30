using System.Text.RegularExpressions;
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig.DocumentLayoutAnalysis;

namespace affolterNET.TextExtractor.Core.Services;

public class FooterService: IFooterService
{
    private readonly IOutput _log;
    private readonly double _pgNumMaxY;
    private readonly Regex _pageNrAndTotalRx;

    public FooterService(IOutput log, double pgNumMaxY = 45)
    {
        _log = log;
        _pgNumMaxY = pgNumMaxY;
        _pageNrAndTotalRx = new Regex(@"[\d]+\s*\/\s*[\d]+");
    }
    
    public void FindFooters(IPdfDoc pdfDoc)
    {
        foreach (var page in pdfDoc.Pages)
        {
            var footer = page.Blocks.LastOrDefault(tb => IsPageNumber(tb, pdfDoc.Pages.Count));
            if (footer != null)
            {
                // AddFooter(footer, page);
            }
            // page.Blocks.RemoveAll(b => b.BlockType == EnumBlockType.Footer);
        }
    }

    private bool IsPageNumber(IPdfTextBlock tb, int pageCount)
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

    // private void AddFooter(IPdfTextBlock block, IPdfPage page)
    // {
    //     block.BlockType = EnumBlockType.Footer;
    //     var pf = new PageFooter(page.PageNr);
    //     pf.AddLines(block.Lines.ToList());
    //     page.Footer = pf;
    // }

    // public void RemovePageNumbers(PdfObjects pdfObj, double pgNumMaxY = 37)
    // {
    //     var pgNumRemoved = RemovePageNumbersWithTotalPages(pdfObj, pgNumMaxY);
    //     if (pgNumRemoved < 1)
    //     {
    //         pgNumRemoved = RemovePageNumbersOnly(pdfObj, pgNumMaxY);
    //     }
    //
    //     _log.Write(EnumLogLevel.Info, "[yellow]", $"Page numbers removed: {pgNumRemoved}", "[/]");
    // }
    
    // private int RemovePageNumbersOnly(PdfObjects pdfObj, double pgNumMaxY)
    // {
    //     var startPoints = new List<int>();
    //     for (var i = 0; i < pdfObj.Pages.Count; i++)
    //     {
    //         var lastWord = pdfObj.Words.Where(w => w.PageNr == i).ToList().LastOrDefault();
    //         if (lastWord == null)
    //         {
    //             _log.Write(EnumLogLevel.Warning, "[orange3]", $"no page number found on page {i}", "[/]");
    //             continue;
    //         }
    //
    //         if (lastWord.Text.IsNumeric())
    //         {
    //             startPoints.Add(pdfObj.Words.IndexOf(lastWord));
    //         }
    //     }
    //
    //     for (var i = startPoints.Count - 1; i >= 0; i--)
    //     {
    //         var pgNum = pdfObj.Words[startPoints[i]];
    //         if (!pgNum.IsBelowY(pgNumMaxY))
    //         {
    //             _log.Write(EnumLogLevel.Warning, $"page number \"{pgNum.Text}\" is not below Y = {pgNumMaxY}");
    //             continue;
    //         }
    //         pdfObj.Words.Remove(pgNum);
    //     }
    //
    //     return startPoints.Count;
    // }

    // private int RemovePageNumbersWithTotalPages(PdfObjects pdfObj, double pgNumMaxY)
    // {
    //     var startPoints = new List<int>();
    //     for (var i = 0; i < pdfObj.Words.Count - 2; i++)
    //     {
    //         if (pdfObj.Words[i].Text.IsNumeric() && pdfObj.Words[i + 1].Text == "/" && pdfObj.Words[i + 2].Text.IsNumeric())
    //         {
    //             if (int.TryParse(pdfObj.Words[i + 2].Text, out var pgCount) && pgCount >= pdfObj.Pages.Count)
    //             {
    //                 startPoints.Add(i);
    //             }
    //         }
    //     }
    //
    //     for (var i = startPoints.Count - 1; i >= 0; i--)
    //     {
    //         var firstOccurrence = startPoints[i];
    //         var pgNumTotal = pdfObj.Words[firstOccurrence + 2];
    //         var slash = pdfObj.Words[firstOccurrence + 1];
    //         var pgNum = pdfObj.Words[firstOccurrence];
    //         if (!pgNumTotal.IsBelowY(pgNumMaxY) || !slash.IsBelowY(pgNumMaxY) || !pgNum.IsBelowY(pgNumMaxY))
    //         {
    //             _log.Write(EnumLogLevel.Info, $"some of the found page numbers are not below Y = {pgNumMaxY}");
    //             continue;
    //         }
    //
    //         pdfObj.Words.Remove(pgNumTotal);
    //         pdfObj.Words.Remove(slash);
    //         pdfObj.Words.Remove(pgNum);
    //     }
    //
    //     return startPoints.Count;
    // }
}

public interface IFooterService
{
    void FindFooters(IPdfDoc pdfDoc);
}
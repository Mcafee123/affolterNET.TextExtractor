using System.Text;
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfDoc : IPdfDoc
{
    private readonly PdfDocument _document;

    public PdfDoc(string fileName, PdfDocument document)
    {
        Filename = fileName;
        _document = document;
    }
    
    public string Filename { get; }
    public List<IPdfPage> Pages { get; set; } = new();
    // public List<IWordOnPage> Words => Pages.SelectMany(p => p.Words).ToList();
    public List<Footnote> Footnotes { get; set; } = new();
    // public List<Footnote> FootnotesWithoutInlineWords { get; set; } = new();
    public FontSizeSettings? FontSizes { get; set; }

    public void GetPages()
    {
        Pages.Clear();
        var pages = _document.GetPages().ToList();
        foreach (var page in pages)
        {
            var pg = new PdfPage(page, this);
            Pages.Add(pg);
        }
    }
    
    public bool Verify(out string message)
    {
        var success = true;
        var sb = new StringBuilder();
        if (!VerifyBlocks(out var blocksErrors))
        {
            sb.Append(blocksErrors);
            success = false;
        }
    
        if (!VerifyFootnotes(out var footnoteErrors))
        {
            sb.Append(footnoteErrors);
            success = false;
        }
    
        message = sb.ToString();
        return success;
    }

    public bool VerifyBlocks(out string blocksErrors)
    {
        var success = true;
        var sb = new StringBuilder();
        foreach (var page in Pages)
        {
            if (!page.VerifyBlocks(out var pageErrors))
            {
                sb.Append(pageErrors);
                success = false;
            }
        }
        blocksErrors = sb.ToString();
        return success;
    }

    public bool VerifyFootnotes(out string message)
    {
        var success = true;
        var sb = new StringBuilder();
        
        // check for double footnotes
        var multipleIds = Footnotes.GroupBy(f => f.Id).Where(g => g.Count() > 1).ToList();
        foreach (var n in multipleIds)
        {
            sb.AppendLine($"Footnote {n} found more than once");
            success = false;
        }
        
        // check for missing footnotes
        var numericFootnotes = Footnotes.Where(f => f.Id.IsNumeric()).ToList();
        var missing = Enumerable.Range(1, numericFootnotes.Distinct().Count()).Except(numericFootnotes.Select(f => int.Parse(f.Id))).ToList();
        foreach (var miss in missing)
        {
            sb.AppendLine($"Footnote {miss} not found");
            success = false;
        }
        
        // check footnote contents
        foreach (var fn in Footnotes)
        {
            if (!fn.BottomContents.Lines.Any())
            {
                sb.AppendLine($"Footnote {fn.Id} has no BottomContents");
                success = false;
            }
        }
        
        // check inline words
        foreach (var fn in Footnotes)
        {
            if (fn.InlineWords.Count < 1)
            {
                sb.AppendLine($"Inline Footnote {fn.Id} not found");
                success = false;
            }
        }

        message = sb.ToString();
        return success;
    }
    
    public void ToJson(string path, IOutput log)
    {
        this.Serialize(path, log);
    }

    public void Dispose()
    {
        _document.Dispose();
    }

    public void RemoveByWordIds(IEnumerable<int> wordIds)
    {
        foreach (var page in Pages)
        {
            var blocksToRemove = new List<IPdfTextBlock>();
            foreach (var block in page.Blocks.TextBlocks)
            {
                var linesToRemove = new List<LineOnPage>();
                foreach (var line in block.Lines)
                {
                    var wordsToRemove = line.Where(word => wordIds.Contains(word.Id)).ToList();
                    line.RemoveAll(wordsToRemove);
                    if (line.All(w => !w.HasText))
                    {
                        linesToRemove.Add(line);
                    }
                }
                block.Lines.RemoveAll(linesToRemove);
                if (block.Lines.Count == 0)
                {
                    blocksToRemove.Add(block);
                }
            }

            foreach (var blockToRemove in blocksToRemove)
            {
                page.Blocks.Remove(blockToRemove);
            }
        }
    }
}

public interface IPdfDoc : IDisposable
{
    string Filename { get; }
    List<IPdfPage> Pages { get; set; }
    // List<IWordOnPage> Words { get; }
    FontSizeSettings? FontSizes { get; set; }
    List<Footnote> Footnotes { get; set; }
    // List<Footnote> FootnotesWithoutInlineWords { get; set; }
    void GetPages();
    void ToJson(string path, IOutput log);
    bool Verify(out string message);
    void RemoveByWordIds(IEnumerable<int> wordIds);
}
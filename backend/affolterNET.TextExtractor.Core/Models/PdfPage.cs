using System.Text;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfPage : IPdfPageAccess
{
    private static int _id = 0;
    private List<IWordOnPage> _words = new();
    public PdfPage(Page page)
    {
        Page = page;
    }

    public Page Page { get; }
    public PdfRectangle BoundingBox => Page.CropBox.Bounds;
    public Page.Experimental ExperimentalAccess => Page.ExperimentalAccess;
    public CropBox CropBox => Page.CropBox;
    public int Nr => Page.Number;
    public IEnumerable<IWordOnPage> Words {
        get
        {
            if (Blocks.TextBlocks.Count  > 0)
            {
                return Blocks.TextBlocks.SelectMany(b => b.Words).ToList();
            }
            if (Lines.Count > 0)
            {
                return Lines.Words.ToList();
            }

            return _words;
        }
    }
    public PdfLines Lines { get; } = new();
    public PdfBlocks Blocks { get; set; } = new();

    public void AddWord(IWordOnPage word)
    {
        word.Id = _id++;
        _words.Add(word);
    }

    public bool RemoveWord(IWordOnPage word)
    {
        return _words.Remove(word);
    }
    
     public bool VerifyBlocks(out string message)
     {
         var success = true;
         var sb = new StringBuilder(); 
         foreach (var block in Blocks.TextBlocks)
         {
             if (block.FirstLine == null)
             {
                 sb.AppendLine($"page {Nr} contains an empty block");
                 success = false;
             }
         }
     
         message = sb.ToString();
         return success;
     }
}
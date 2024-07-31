using System.Text;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfPage : IPdfPageAccess
{
#pragma warning disable CS0414 // Field is assigned but its value is never used
    private static int _id = 0;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    public PdfPage(Page page, IPdfDoc doc)
    {
        Page = page;
        Document = doc;
    }

    public IPdfDoc Document { get; }
    public Page Page { get; }
    public PdfRectangle BoundingBox => Page.CropBox.Bounds;
    public Page.Experimental ExperimentalAccess => Page.ExperimentalAccess;
    public CropBox CropBox => Page.CropBox;
    public int Nr => Page.Number;
    public PdfBlocks Blocks { get; set; } = new();
    
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
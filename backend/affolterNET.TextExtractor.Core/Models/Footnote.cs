using affolterNET.TextExtractor.Core.Models.Interfaces;

namespace affolterNET.TextExtractor.Core.Models;

public class Footnote
{
    public Footnote(string id, IPdfPage page)
    {
        Id = id;
        BottomContents = new PdfTextBlock(page);
    }

    public Footnote(IPdfPage page, IWordOnPage inlineWord, IWordOnPage footnoteWord)
    {
        Id = inlineWord.Text;
        InlineWords.Add(inlineWord);
        BottomContents = new PdfTextBlock(page);
    }
    
    public List<IWordOnPage> InlineWords { get; set; } = new();
    public string Id { get; }
    public IPdfTextBlock BottomContents { get; }
    public LineOnPage? BottomContentsCaption { get; set; }

    public override string ToString()
    {
        return $"{Id}: {BottomContents}";
    }

    public bool Contains(IWordOnPage word)
    {
        return InlineWords.Any(w => w == word) || 
               (BottomContentsCaption?.Any(w => w == word) ?? false) ||
               BottomContents.Words.Any(w => w == word);
    }
}
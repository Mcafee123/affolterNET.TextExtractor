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
        FootnoteWords.Add(footnoteWord);
        BottomContents = new PdfTextBlock(page);
    }

    public List<IWordOnPage> FootnoteWords { get; set; } = new();
    public List<IWordOnPage> InlineWords { get; set; } = new();
    public string Id { get; }
    public IPdfTextBlock BottomContents { get; }
    public LineOnPage? BottomContentsCaption { get; set; }

    public override string ToString()
    {
        return $"{Id}: {BottomContents}";
    }
}
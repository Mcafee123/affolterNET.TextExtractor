namespace affolterNET.TextExtractor.Core.Models;

public class Footnote
{
    public Footnote(string id, IPdfTextBlock bottomContents)
    {
        Id = id;
        BottomContents = bottomContents;
    }

    public Footnote(IWordOnPage inlineWord)
    {
        Id = inlineWord.Text;
        InlineWords.Add(inlineWord);
        BottomContents = new PdfTextBlock();
    }

    public List<IWordOnPage> InlineWords { get; set; } = new();
    public string Id { get; }
    public IPdfTextBlock BottomContents { get; }

    public override string ToString()
    {
        return $"{Id}: {BottomContents}";
    }
}
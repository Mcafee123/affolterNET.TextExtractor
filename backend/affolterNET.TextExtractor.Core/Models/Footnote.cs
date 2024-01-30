namespace affolterNET.TextExtractor.Core.Models;

public class Footnote
{
    public Footnote(int number, IPdfTextBlock bottomContents, bool isStar = false)
    {
        Number = number;
        BottomContents = bottomContents;
        IsStar = isStar;
    }

    public List<IWordOnPage> InlineWords { get; set; } = new();
    public int Number { get; }
    public IPdfTextBlock BottomContents { get; }
    public bool IsStar { get; }

    public override string ToString()
    {
        return $"{Number}: {BottomContents}";
    }
}
using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IFootnoteDetector
{
    List<Footnote> DetectBottomFootnotes(IPdfPage page, List<Footnote> fn, double mainFontSize);
    List<Footnote> DetectInlineFootnotes(IPdfTextBlock block, double mainFontSize, double minBaseLineDiff);
}
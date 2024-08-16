using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IFootnoteDetector
{
    List<Footnote> DetectFootnotes(IPdfDoc document, double minBaselineDiff);
}
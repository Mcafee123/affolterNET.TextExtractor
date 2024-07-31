using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IFootnoteDetector
{
    List<Footnote> DetectFootnotes(IPdfDoc document, double minBaselineDiff);
}
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IFootnoteDetector
{
    List<Footnote> DetectBottomFootnotes(IPdfPage page, List<Footnote> fn, double mainFontSize, DetectFootnotesStep.DetectFootnotesStepSettings settings);
    List<Footnote> DetectInlineFootnotes(IPdfTextBlock block, double mainFontSize, double minBaseLineDiff);
}
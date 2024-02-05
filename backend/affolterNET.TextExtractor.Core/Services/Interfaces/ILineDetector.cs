using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface ILineDetector
{
    PdfLines DetectLines(List<IWordOnPage> inputWords, double baseLineMatchingRange);
}
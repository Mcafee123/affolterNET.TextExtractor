using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services;

public interface ILineDetector
{
    PdfLines DetectLines(List<IWordOnPage> inputWords, int maxPagesToConsider = int.MaxValue);
}
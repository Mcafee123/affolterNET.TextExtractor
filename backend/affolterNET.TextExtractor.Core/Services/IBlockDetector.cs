using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services;

public interface IBlockDetector
{
    IPdfTextBlocks FindBlocks(IPdfPage page, double rangeY = 1, double blockVerticalTolerance = 10);
    IPdfTextBlocks FindHorizontalBlocks(IPdfTextBlocks blocks, double blockHorizontalTolerance = 10);
}
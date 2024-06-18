using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IBlockDetector
{
    void FindBlocks(IPdfPage page, FontSizeSettings fontSizeSettings, double horizontalDistDiff,
        double blockOverlapDistanceDiff, double baseLineMatchingRange, double quadtreeBlockResolution);
}
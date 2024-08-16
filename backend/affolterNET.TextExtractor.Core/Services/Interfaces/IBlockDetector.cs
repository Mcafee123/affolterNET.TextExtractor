using affolterNET.TextExtractor.Core.Models.Interfaces;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IBlockDetector
{
    void FindBlocks(IPdfPage page, double verticalBlockDistanceDiffFactor,
        double blockOverlapDistanceDiff, double baseLineMatchingRange, double quadtreeBlockResolution);
}
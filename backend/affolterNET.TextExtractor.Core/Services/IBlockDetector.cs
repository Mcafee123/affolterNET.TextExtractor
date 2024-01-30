using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services;

public interface IBlockDetector
{
    IPdfTextBlocks FindBlocks(IPdfPage page, double topDistanceRelation);
}
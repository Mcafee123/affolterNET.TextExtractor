using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IBlockDetector
{
    IPdfBlocks FindBlocks(IPdfPage page, FontSizeSettings fontSizeSettings, double newBlockDistanceDiff, double blockOverlapDistanceDiff);
}
using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IPageNumberService
{
    int DetectPageNumberBlocks(IPdfDoc doc);
}
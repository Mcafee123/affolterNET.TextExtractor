using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IHeaderService
{
    public int DetectHeaders(IPdfDoc contextDocument);
}
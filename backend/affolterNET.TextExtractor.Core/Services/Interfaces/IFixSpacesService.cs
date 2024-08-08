using affolterNET.TextExtractor.Core.Models;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IFixSpacesService
{
    int FixSpaces(IPdfDoc doc, double baseLineMatchingRange);
}
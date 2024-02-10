namespace affolterNET.TextExtractor.Core.Models.Interfaces;

public interface IPdfBlock: IPdfBlockBase
{
    IPdfPage Page { get; }
}
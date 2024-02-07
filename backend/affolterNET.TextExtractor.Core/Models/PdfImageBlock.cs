using affolterNET.TextExtractor.Core.Models.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.XObjects;

namespace affolterNET.TextExtractor.Core.Models;

public class PdfImageBlock: IPdfImageBlock
{
    private readonly IPdfImage _image;

    public PdfImageBlock(IPdfPage page, IPdfImage image)
    {
        Page = page;
        _image = image;
        if (!image.TryGetPng(out var imageRawBytes))
        {
            // jpeg
            Base64Image = $"data:image/jpeg;base64,{Convert.ToBase64String(image.RawBytes.ToArray())}";
        }
        else
        {
            // png
            Base64Image = $"data:image/png;base64,{Convert.ToBase64String(imageRawBytes.ToArray())}";
        }

        var type = string.Empty;
        switch (image)
        {
            case XObjectImage ximg:
                type = "XObject";
                break;
            case InlineImage inline:
                type = "Inline";
                break;
        }

        ImgType = type;
    }

    public string ImgType { get; set; }

    public double TopDistance { get; set; }
    public PdfRectangle BoundingBox => _image.Bounds;
    public IPdfPage Page { get; set; }
    public string Base64Image { get; }
}
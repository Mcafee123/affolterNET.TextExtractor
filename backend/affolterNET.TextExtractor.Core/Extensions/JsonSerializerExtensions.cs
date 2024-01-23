using System.Text.Json;
using System.Text.Json.Serialization;
using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class JsonSerializerExtensions
{
    private static readonly JsonSerializerOptions _options;

    static JsonSerializerExtensions()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _options.Converters.Add(new PdfRectangleConverter());
    }

    public static T Deserialize<T>(this string path)
    {
        var json = File.ReadAllText(path);
        var obj = JsonSerializer.Deserialize<T>(json, _options);
        return obj!;
    }
    
    public static void Serialize(this IPdfDoc pdfDoc, string path)
    {
        var toSerialize = new PdfDocJson(pdfDoc);
        toSerialize.Serialize(path);
    }
    
    public static void Serialize(this List<IWordOnPage> words, string path)
    {
        words.ForEach(w => w.Line = null);
        var toSerialize = new List<WordOnPageJson>();
        toSerialize.AddRange(words.Select(w => new WordOnPageJson(w)));
        toSerialize.Serialize(path);
    }
    
    private static void Serialize(this object obj, string path)
    {
        var json = JsonSerializer.Serialize(obj, _options);
        File.WriteAllTextAsync(path, json).GetAwaiter().GetResult();
    }
}

public class PdfRectangleConverter : JsonConverter<PdfRectangle>
{
    private const string BottomLeftX = "bottomLeftX";
    private const string BottomLeftY = "bottomLeftY";
    private const string TopRightX = "topRightX";
    private const string TopRightY = "topRightY";
    public override PdfRectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        double bottomLeftX = default;
        double bottomLeftY = default;
        double topRightX = default;
        double topRightY = default;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new PdfRectangle(new PdfPoint(bottomLeftX, bottomLeftY), new PdfPoint(topRightX, topRightY));
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case nameof(BottomLeftX):
                        bottomLeftX = reader.GetDouble();
                        break;
                    case nameof(BottomLeftY):
                        bottomLeftY = reader.GetDouble();
                        break;
                    case nameof(TopRightX):
                        topRightX = reader.GetDouble();
                        break;
                    case nameof(TopRightY):
                        topRightY = reader.GetDouble();
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, PdfRectangle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(BottomLeftX, value.BottomLeft.X);
        writer.WriteNumber(BottomLeftY, value.BottomLeft.Y);
        writer.WriteNumber(TopRightX, value.TopRight.X);
        writer.WriteNumber(TopRightY, value.TopRight.Y);
        writer.WriteEndObject();
    }
}

public class PdfDocJson
{
    public PdfDocJson()
    {
        
    }

    public PdfDocJson(IPdfDoc pdfDoc)
    {
        Filename = pdfDoc.Filename;
        foreach (var page in pdfDoc.Pages)
        {
            Pages.Add(new PdfPageJson(page));
        }
    }

    public string Filename { get; set; } = null!;
    public List<PdfPageJson> Pages { get; set; } = new();
}

public class PdfPageJson
{
    public PdfPageJson()
    {
        
    }

    public PdfPageJson(IPdfPage page)
    {
        Nr = page.Nr;
        BoundingBox = page.BoundingBox;
        foreach (var block in page.Blocks)
        {
            Blocks.Add(new PdfBlockJson(block));
        }
    }

    public int Nr { get; set; }
    public PdfRectangle BoundingBox { get; set; }
    public List<PdfBlockJson> Blocks { get; set; } = new();
}

public class PdfBlockJson
{
    public PdfBlockJson()
    {
        
    }

    public PdfBlockJson(IPdfTextBlock block)
    {
        BoundingBox = block.BoundingBox;
        foreach (var line in block.Lines)
        {
            Lines.Add(new PdfLineJson(line));
        }
    }

    public PdfRectangle BoundingBox { get; set; }
    public List<PdfLineJson> Lines { get; set; } = new();
}

public class PdfLineJson
{
    public PdfLineJson()
    {
        
    }
    
    public PdfLineJson(LineOnPage line)
    {
        foreach (var word in line)
        {
            Words.Add(new WordOnPageJson(word));
        }

        BoundingBox = line.BoundingBox;
    }

    public PdfRectangle BoundingBox { get; set; }
    public List<WordOnPageJson> Words { get; set; } = new();
}

public class WordOnPageJson
{
    public WordOnPageJson()
    {
        
    }
    
    public WordOnPageJson(IWordOnPage word)
    {
        BoundingBox = word.BoundingBox;
        Text = word.Text;
    }

    public PdfRectangle BoundingBox { get; set; }
    public string Text { get; set; } = null!;
    public int PageNr { get; set; }
}
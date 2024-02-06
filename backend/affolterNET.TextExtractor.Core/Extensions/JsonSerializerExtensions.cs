using System.Text.Json;
using System.Text.Json.Serialization;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using UglyToad.PdfPig.Content;
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
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
        _options.Converters.Add(new PdfPointConverter());
        _options.Converters.Add(new PdfRectangleConverter());
    }

    public static T Deserialize<T>(this string path)
    {
        var json = File.ReadAllText(path);
        var obj = JsonSerializer.Deserialize<T>(json, _options);
        return obj!;
    }
    
    public static void Serialize(this IPdfDoc pdfDoc, string path, IOutput log)
    {
        var toSerialize = new PdfDocJson(pdfDoc, log);
        toSerialize.Serialize(path);
    }
    
    public static string Serialize(this IPdfDoc pdfDoc, IOutput log)
    {
        var toSerialize = new PdfDocJson(pdfDoc, log);
        var json = JsonSerializer.Serialize(toSerialize, _options);
        return json;
    }
    
    private static void Serialize(this object obj, string path)
    {
        var json = JsonSerializer.Serialize(obj, _options);
        File.WriteAllTextAsync(path, json).GetAwaiter().GetResult();
    }
}

public class PdfPointConverter : JsonConverter<PdfPoint>
{
    private const string X = "X";
    private const string Y = "Y";
    
    public override PdfPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        double X = default;
        double Y = default;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new PdfPoint(X, Y);
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case nameof(X):
                        X = reader.GetDouble();
                        break;
                    case nameof(Y):
                        Y = reader.GetDouble();
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, PdfPoint value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(X, value.X);
        writer.WriteNumber(Y, value.Y);
        writer.WriteEndObject();
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

    public PdfDocJson(IPdfDoc pdfDoc, IOutput log)
    {
        Filename = pdfDoc.Filename;
        FontNames = string.Join(", ", pdfDoc.FontSizes?.AllFontNames ?? new List<string>());
        FontGroups = pdfDoc.FontSizes?
            .Select(fs => $"{fs.GroupId + 1}: {Math.Round(fs.AvgFontSize, 2):##.00} (Words: {fs.WordCount}, Min: {Math.Round(fs.MinFontSize, 2):##.00}, Max: {Math.Round(fs.MaxFontSize, 2):##.00})")
            .ToList() ?? new List<string>();
        foreach (var page in pdfDoc.Pages)
        {
            Pages.Add(new PdfPageJson(page, log));
        }

        foreach (var footnote in pdfDoc.Footnotes)
        {
            Footnotes.Add(new PdfFootnoteJson(footnote, log));
        }
    }

    public List<string> FontGroups { get; set; } = new();
    public string Filename { get; set; } = null!;
    public string FontNames { get; set; } = null!;
    public List<PdfPageJson> Pages { get; set; } = new();
    public List<PdfFootnoteJson> Footnotes { get; set; } = new();
}

public class PdfFootnoteJson
{
    public PdfFootnoteJson()
    {
        
    }

    public PdfFootnoteJson(Footnote footnote, IOutput log)
    {
        Id = footnote.Id;
        foreach (var w in footnote.InlineWords)
        {
            InlineWords.Add(new WordOnPageJson(w, log));
        }

        BottomContents = new PdfBlockJson(footnote.BottomContents, log);
        if (footnote.BottomContentsCaption != null)
        {
            BottomContentsCaption = new PdfLineJson(footnote.BottomContentsCaption, log);
        }
        else
        {
            log.Write(EnumLogLevel.Error, $"Footnote {footnote.Id}: BottomContentsCaption is null");
        }
    }

    public string Id { get; set; } = null!;
    public List<WordOnPageJson> InlineWords { get; set; } = new();
    public PdfBlockJson BottomContents { get; set; } = new();
    public PdfLineJson? BottomContentsCaption { get; set; }
}

public class PdfPageJson
{
    public PdfPageJson()
    {
        
    }

    public PdfPageJson(IPdfPage page, IOutput log)
    {
        Nr = page.Nr;
        BoundingBox = page.BoundingBox;
        foreach (var block in page.Blocks)
        {
            Blocks.Add(new PdfBlockJson(block, log));
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

    public PdfBlockJson(IPdfTextBlock block, IOutput log)
    {
        BoundingBox = block.BoundingBox;
        foreach (var line in block.Lines)
        {
            Lines.Add(new PdfLineJson(line, log));
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
    
    public PdfLineJson(LineOnPage line, IOutput log)
    {
        foreach (var word in line)
        {
            Words.Add(new WordOnPageJson(word, log));
        }

        BoundingBox = line.BoundingBox;
        FontSizeAvg = line.FontSizeAvg;
        TopDistance = line.TopDistance;
        BaseLineY = line.BaseLineY;
    }

    public double BaseLineY { get; set; }
    public double TopDistance { get; set; }
    public double FontSizeAvg { get; set; }
    public PdfRectangle BoundingBox { get; set; }
    public List<WordOnPageJson> Words { get; set; } = new();
}

public class WordOnPageJson
{
    public WordOnPageJson()
    {
        
    }
    
    public WordOnPageJson(IWordOnPage word, IOutput log)
    {
        Id = word.Id;
        BoundingBox = word.BoundingBox;
        BaseLineY = word.BaseLineY;
        Text = word.Text;
        FontName = word.FontName;
        Orientation = word.TextOrientation.ToString();
        foreach (var letter in word.Letters)
        {
            Letters.Add(new LetterJson(letter, log));
        }
    }

    public int Id { get; set; }
    public double BaseLineY { get; set; }
    public PdfRectangle BoundingBox { get; set; }
    public string Text { get; set; } = null!;
    public int PageNr { get; set; }
    public string FontName { get; set; } = null!;
    public string? Orientation { get; set; }
    public List<LetterJson> Letters { get; set; } = new();
}

public class LetterJson
{
    public LetterJson()
    {
        
    }

    public LetterJson(Letter letter, IOutput log)
    {
        GlyphRectangle = letter.GlyphRectangle;
        StartBaseLine = letter.StartBaseLine;
        EndBaseLine = letter.EndBaseLine;
        Text = letter.Value;
        FontSize = letter.PointSize;
        IsBold = letter.Font.IsBold;
        IsItalic = letter.Font.IsItalic;
        Orientation = letter.TextOrientation.ToString();
    }

    public PdfPoint EndBaseLine { get; set; }
    public PdfPoint StartBaseLine { get; set; }
    public bool IsItalic { get; set; }
    public bool IsBold { get; set; }
    public string? Orientation { get; set; }
    public PdfRectangle GlyphRectangle { get; set; }
    public string Text { get; set; } = null!;
    public double FontSize { get; set; }
}
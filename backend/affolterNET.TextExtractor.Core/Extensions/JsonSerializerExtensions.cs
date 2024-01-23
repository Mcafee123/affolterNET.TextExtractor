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
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new PdfRectangleConverter());
    }

    public static T Deserialize<T>(this string path)
    {
        var json = File.ReadAllText(path);
        var obj = JsonSerializer.Deserialize<T>(json, _options);
        return obj!;
    }

    public static void Serialize(this PdfLines lines, string path)
    {
        lines.SelectMany(l => l).ToList().ForEach(w => w.Line = null);
        lines.ToList().ForEach(l => l.Lines = null);
        var toSerialize = new LawLinesJson(lines);
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
    private const string BottomLeftX = "BottomLeftX";
    private const string BottomLeftY = "BottomLeftY";
    private const string TopRightX = "TopRightX";
    private const string TopRightY = "TopRightY";
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


public class LawLinesJson
{
    public LawLinesJson()
    {
        
    }
    
    public LawLinesJson(PdfLines lines)
    {
        foreach (var line in lines)
        {
            Lines.Add(new LawLineJson(line));
        }
    }

    public List<LawLineJson> Lines { get; set; } = new();
}

public class LawLineJson
{
    public LawLineJson()
    {
        
    }
    
    public LawLineJson(LineOnPage line)
    {
        foreach (var word in line)
        {
            Words.Add(new WordOnPageJson(word));
        }
    }

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
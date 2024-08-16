using System.Text.Json;
using System.Text.Json.Serialization;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Models.Interfaces;
using affolterNET.TextExtractor.Core.Models.JsonModels;
using UglyToad.PdfPig.Core;

namespace affolterNET.TextExtractor.Core.Extensions;

public static class JsonSerializerExtensions
{
    public static JsonSerializerOptions ConfigureJsonSerializerOptions(this JsonSerializerOptions options)
    {
        options.WriteIndented = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.Converters.Add(new PdfPointConverter());
        options.Converters.Add(new PdfRectangleConverter());
        return options;
    }
    private static readonly JsonSerializerOptions Options;
    
    static JsonSerializerExtensions()
    {
        Options = new JsonSerializerOptions().ConfigureJsonSerializerOptions();
    }

    public static T Deserialize<T>(this string path)
    {
        var json = File.ReadAllText(path);
        var obj = JsonSerializer.Deserialize<T>(json, Options);
        return obj!;
    }
    
    public static T DeserializeFromString<T>(this string content)
    {
        var obj = JsonSerializer.Deserialize<T>(content, Options);
        return obj!;
    }
    
    public static void SerializePdfDoc(this IPdfDoc pdfDoc, string path, IOutput log)
    {
        var toSerialize = new PdfDocJson(pdfDoc, null, true, log);
        toSerialize.SerializeAndSave(path);
    }
    
    public static void SerializePdfPage(this IPdfPage pdfPage, string path, IOutput log)
    {
        var toSerialize = new PdfPageJson(pdfPage, log);
        toSerialize.SerializeAndSave(path);
    }
    
    public static void SerializeAndSave<T>(this T toSerialize, string path) where T: IJsonSaveable
    {
        var json = JsonSerializer.Serialize(toSerialize, Options);
        File.WriteAllTextAsync(path, json).GetAwaiter().GetResult();
    }
    
    public static Stream Serialize<T>(this T toSerialize) where T: IJsonSaveable
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, toSerialize, Options);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
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
        
        double x = default;
        double y = default;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new PdfPoint(x, y);
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                if (propertyName == X)
                {
                    x = reader.GetDouble();
                }
                else if (propertyName == Y)
                {
                    y = reader.GetDouble();
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
                if (propertyName == BottomLeftX)
                {
                    bottomLeftX = reader.GetDouble();
                }
                else if (propertyName == BottomLeftY)
                {
                    bottomLeftY = reader.GetDouble();
                }
                else if (propertyName == TopRightX)
                {
                    topRightX = reader.GetDouble();
                }
                else if (propertyName == TopRightY)
                {
                    topRightY = reader.GetDouble();
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
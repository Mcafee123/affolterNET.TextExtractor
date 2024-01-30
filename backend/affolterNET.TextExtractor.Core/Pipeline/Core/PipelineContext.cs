using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Pipeline.Core;

public class PipelineContext: IPipelineContext
{
    private List<IStepSettings> _settings = new();

    public PipelineContext(string filename)
    {
        Filename = filename;
    }
    
    public PipelineContext(Stream pdfStream, string filename): this(filename) 
    {
        PdfStream = pdfStream;
    }
    
    public string Filename { get; }
    public List<Word> OriginalWords { get; } = new();
    public IPdfDoc? Document { get; private set; }

    public void SetDocument(PdfDocument document)
    {
        Document = new PdfDoc(Filename, document);
    }

    public Stream? PdfStream { get; } = null;

    public T GetSettings<T>() where T : class, IStepSettings, new()
    {
        var settings = _settings.FirstOrDefault(s => s.GetType() == typeof(T)) as T;
        if (settings == null)
        {
            settings = new T();
            _settings.Add(settings);
        }

        return settings;
    }

    public void AddSettings<T>(T settings) where T : class, IStepSettings, new()
    {
        _settings.Add(settings);
    }

    public override string ToString()
    {
        var pgCount = Document == null ? 0 : Document.Pages.Count;
        return $"{Filename}; Pages: {pgCount}";
    }

    public void Dispose()
    {
        Document?.Dispose();
    }
}
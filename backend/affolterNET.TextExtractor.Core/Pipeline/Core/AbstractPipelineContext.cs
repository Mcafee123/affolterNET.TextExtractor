using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace affolterNET.TextExtractor.Core.Pipeline.Core;

public abstract class AbstractPipelineContext: IPipelineContext
{
    private List<IStepSettings> _settings = new();

    public AbstractPipelineContext(string filename)
    {
        Filename = filename;
    }
    
    public AbstractPipelineContext(Stream pdfStream, string filename): this(filename) 
    {
        PdfStream = pdfStream;
    }
    
    public string Filename { get; }
    public List<Word> OriginalWords { get; } = new();
    public List<IPdfImage> OriginalImages { get; }= new();
    public string? TextContent { get; set; }
    public IPdfDoc? Document { get; private set; }
    public abstract void SetDocument(PdfDocument document);

    protected void SetDocument(IPdfDoc document)
    {
        Document = document;
    }

    public Stream? PdfStream { get; } = null;

    public TSettings GetSettings<TSettings>() where TSettings : class, IStepSettings, new()
    {
        var settings = _settings.FirstOrDefault(s => s.GetType() == typeof(TSettings)) as TSettings;
        if (settings == null)
        {
            settings = new TSettings();
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
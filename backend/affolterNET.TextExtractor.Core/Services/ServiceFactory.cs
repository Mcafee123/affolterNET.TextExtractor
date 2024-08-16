using affolterNET.TextExtractor.Core.Pipeline.Interfaces;

namespace affolterNET.TextExtractor.Core.Services;

public class ServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IBasicPdfPipeline CreatePipeline(string fileName)
    {
        var loggerContextProvider = GetService<LoggerContextProvider>();
        loggerContextProvider.AddContext(fileName);
        var pipeline = GetService<IBasicPdfPipeline>();
        return pipeline;
    }

    public T GetService<T>() where T: class
    {
        var svc = _serviceProvider.GetService(typeof(T)) as T;
        if (svc == null)
        {
            throw new TypeLoadException($"Could not load {nameof(T)}");
        }
        return svc;
    }
}
using affolterNET.TextExtractor.Core.Configuration;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace affolterNET.TextExtractor.Storage.Extensions;

public static class ConfigureServicesExtension
{
    public static void AddTextExtractorStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind StorageOptions from configuration
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

        // Register storage services
        services.AddTransient<BlobStorageService>();
        services.AddTransient<IExtractorFileService, ExtractorFileService>();
    }
}
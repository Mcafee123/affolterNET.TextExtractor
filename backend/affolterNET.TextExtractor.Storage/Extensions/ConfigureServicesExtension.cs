using System.Configuration;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Storage.Models;
using affolterNET.TextExtractor.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace affolterNET.TextExtractor.Storage.Extensions;

public static class ConfigureServicesExtension
{
    public static void AddTextExtractorStorageServices(this IServiceCollection services, IConfiguration config)
    {
        var storageAccountName = config.GetValue<string>("StorageAccountName");
        if (string.IsNullOrWhiteSpace(storageAccountName))
        {
            throw new ConfigurationErrorsException("StorageAccountName is missing in the configuration");
        }

        var storageAccountKey = config.GetValue<string>("StorageAccountKey");
        if (string.IsNullOrWhiteSpace(storageAccountKey))
        {
            throw new ConfigurationErrorsException("StorageAccountKey is missing in the configuration");
        }

        services.AddSingleton<StorageAccountName>(_ => new StorageAccountName(storageAccountName));
        services.AddSingleton<StorageAccountKey>(_ => new StorageAccountKey(storageAccountKey));
        services.AddTransient<BlobStorageService>();
        services.AddTransient<IExtractorFileService, ExtractorFileService>();
    }
}
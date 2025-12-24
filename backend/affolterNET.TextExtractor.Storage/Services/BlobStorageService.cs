using affolterNET.TextExtractor.Core.Configuration;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace affolterNET.TextExtractor.Storage.Services;

public class BlobStorageService
{
    private readonly IOutput _log;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(IOptions<StorageOptions> storageOptions, IOutput log)
    {
        _log = log;
        var options = storageOptions.Value;

        // Priority 1: Connection string (local development with Azurite or production connection string)
        if (!string.IsNullOrEmpty(options.ConnectionString))
        {
            _log.Write(EnumLogLevel.Debug, $"Using connection string authentication for blob storage");
            _blobServiceClient = new BlobServiceClient(options.ConnectionString);
        }
        // Priority 2: Managed Identity with specific client ID
        else if (!string.IsNullOrEmpty(options.StorageAccountName) && !string.IsNullOrEmpty(options.StorageClientId))
        {
            _log.Write(EnumLogLevel.Debug, $"Using managed identity authentication with client ID for storage account: {options.StorageAccountName}");
            var uri = new Uri($"https://{options.StorageAccountName}.blob.core.windows.net");
            var credential = new ManagedIdentityCredential(options.StorageClientId);
            _blobServiceClient = new BlobServiceClient(uri, credential);
        }
        // Priority 3: Default Azure Credential (Azure CLI, VS, managed identity without specific client ID)
        else if (!string.IsNullOrEmpty(options.StorageAccountName))
        {
            _log.Write(EnumLogLevel.Debug, $"Using default Azure credential for storage account: {options.StorageAccountName}");
            var uri = new Uri($"https://{options.StorageAccountName}.blob.core.windows.net");
            var credential = new DefaultAzureCredential();
            _blobServiceClient = new BlobServiceClient(uri, credential);
        }
        else
        {
            throw new InvalidOperationException(
                "Storage configuration is invalid. Either ConnectionString or StorageAccountName must be configured.");
        }
    }

    public async Task<List<string>> ListBlobContainersAsync()
    {
        var list = new List<string>();
        var containers = _blobServiceClient.GetBlobContainersAsync();
        await foreach (var container in containers)
        {
            list.Add(container.Name);
        }

        return list;
    }

    public async Task<List<BlobHierarchyItem>> GetFolders(string containerName)
    {
        var folders = new List<BlobHierarchyItem>();
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var resultSegment = container.GetBlobsByHierarchyAsync(delimiter: "/").AsPages(default, 100);
        await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
        {
            folders.AddRange(blobPage.Values.Where(bhi => bhi.IsPrefix));
        }

        return folders;
    }

    public async Task<List<BlobHierarchyItem>> GetBlobs(string containerName, string prefix = "")
    {
        var blobs = new List<BlobHierarchyItem>();
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var resultSegment = container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/").AsPages(default, 100);
        await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
        {
            foreach (var bhi in blobPage.Values)
            {
                if (bhi.IsPrefix)
                {
                    var files = await GetBlobs(containerName, bhi.Prefix);
                    blobs.AddRange(files);
                }
                else
                {
                    blobs.Add(bhi);
                }
            }
        }

        return blobs;
    }

    public async Task<string> GetStringBlob(string containerName, string blobName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobclient = container.GetBlobClient(blobName);
        var downloadInfo = await blobclient.DownloadAsync();
        using var streamReader = new StreamReader(downloadInfo.Value.Content);
        var jsonContent = await streamReader.ReadToEndAsync();
        return jsonContent;
    }

    public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        var result = await blob.DeleteIfExistsAsync();
        return result;
    }

    public async Task<BlobContentInfo> UploadFileAsync(string containerName, string path, Stream content)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync();
        var blob = container.GetBlobClient(path);
        var result = await blob.UploadAsync(content);
        return result;
    }

    public async Task DeleteByHierarchy(string containerName, string folder)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var resultSegment = container.GetBlobsByHierarchyAsync(prefix: folder, delimiter:"/").AsPages(default, 100);
            
        await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
        {
            foreach (var bhi in blobPage.Values)
            {
                if (bhi.IsPrefix)
                {
                    await DeleteByHierarchy(containerName, bhi.Prefix);
                }
                else
                {
                    await container.DeleteBlobAsync(bhi.Blob.Name);
                    _log.Write(EnumLogLevel.Debug, $"Blob name: {bhi.Blob.Name} deleted");
                }
            }
        }
    }
}
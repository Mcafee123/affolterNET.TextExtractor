using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models.StorageModels;
using affolterNET.TextExtractor.Storage.Models;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace affolterNET.TextExtractor.Storage.Services;

public class BlobStorageService
{
    private readonly IOutput _log;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(StorageAccountName storageAccountName, StorageAccountKey storageAccountKey, IOutput log)
    {
        _log = log;
        var credential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
        var uri = $"https://{storageAccountName.ToString()}.blob.core.windows.net";
        _blobServiceClient = new BlobServiceClient(new Uri(uri), credential);
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

    public async Task<List<BlobHierarchyItem>> GetBlobs(string containerName, string prefix = "")
    {
        var blobs = new List<BlobHierarchyItem>();
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var resultSegment = container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/").AsPages(default, 100);
        await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
        {
            blobs.AddRange(blobPage.Values);
        }

        return blobs;
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

    public async Task DeleteByHierarchy(string containerName, string dateFolderName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var resultSegment = container.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", $"{dateFolderName}/").AsPages(default, 100);
            
        await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
        {
            foreach (var hierarchyItem in blobPage.Values)
            {
                await container.DeleteBlobAsync(hierarchyItem.Blob.Name);
                _log.Write(EnumLogLevel.Debug, $"Blob name: {hierarchyItem.Blob.Name} deleted");
            }
        }
    }
}
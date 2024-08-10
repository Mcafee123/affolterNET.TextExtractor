using affolterNET.TextExtractor.Core.Helpers;
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
using affolterNET.Web.Core.Configuration;
using affolterNET.Web.Core.Models;
using affolterNET.Web.Core.Options;

namespace affolterNET.TextExtractor.Core.Configuration;

/// <summary>
/// Configuration options for Azure Storage access.
/// Priority:
/// 1. ConnectionString - if set, uses connection string auth (Azurite or production)
/// 2. StorageClientId - if set, uses ManagedIdentityCredential with specific client ID
/// 3. Otherwise - uses DefaultAzureCredential (Azure CLI, VS, etc.)
/// </summary>
public class StorageOptions : IConfigurableOptions<StorageOptions>
{
    public static string SectionName => "affolterNET:TextExtractor:StorageSettings";

    public static StorageOptions CreateDefaults(AppSettings settings)
    {
        return new StorageOptions(settings);
    }

    public void CopyTo(StorageOptions options)
    {
        options.ConnectionString = ConnectionString;
        options.StorageAccountName = StorageAccountName;
        options.StorageClientId = StorageClientId;
    }

    public StorageOptions() : this(new AppSettings())
    {
    }

    private StorageOptions(AppSettings settings)
    {
        // Defaults for local development - use Azurite emulator
        ConnectionString = "";
        StorageAccountName = "";
        StorageClientId = "";
    }

    /// <summary>
    /// Connection string for Azure Storage or Azurite emulator.
    /// If set, this takes priority over managed identity authentication.
    /// Use "UseDevelopmentStorage=true" for local Azurite emulator.
    /// </summary>
    [Sensible]
    public string ConnectionString { get; set; }

    /// <summary>
    /// The name of the Azure Storage account (without .blob.core.windows.net suffix).
    /// Required when using managed identity authentication.
    /// </summary>
    public string StorageAccountName { get; set; }

    /// <summary>
    /// The client ID of the user-assigned managed identity for storage access.
    /// If set, uses ManagedIdentityCredential with this client ID.
    /// If not set (and ConnectionString is also not set), uses DefaultAzureCredential.
    /// </summary>
    public string StorageClientId { get; set; }
}

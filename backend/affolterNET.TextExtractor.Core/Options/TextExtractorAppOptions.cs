using System.Text.Json;
using System.Text.Json.Serialization;
using affolterNET.TextExtractor.Core.Configuration;
using affolterNET.Web.Bff.Options;
using affolterNET.Web.Core.Configuration;
using affolterNET.Web.Core.Extensions;
using affolterNET.Web.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace affolterNET.TextExtractor.Core.Options;

public class BexioAppOptions(AppSettings appSettings, IConfiguration config)
{
    public void Configure(IServiceCollection services, BffAppOptions appOptions)
    {
        var actions = new ConfigureActions();
        actions.Add(ConfigureStorage);
        Storage.RunActions(actions);
        Storage.ConfigureDi(services);
    }
    
    public StorageOptions Storage { get; set; } = config.CreateFromConfig<StorageOptions>(appSettings);
    public Action<StorageOptions>? ConfigureStorage { get; set; }

    public string ToJson()
    {
        var result = new Dictionary<string, object>();
        Storage.AddToConfigurationDict(result);
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Serialize(result, options);
    }

    public void ValidateConfiguration()
    {
        var errors = new List<string>
        {
            // Storage.CheckNullOrWhitespace(x => x.ConnectionString)
        };

        var realErrors = errors.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        if (realErrors.Any())
        {
            throw new ApplicationException("Invalid BexioAppOptions configuration: " + string.Join("\n", realErrors));
        }
    }
}
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(api.Startup))]

namespace api;

public class Startup : FunctionsStartup
{
    // to configure logging: https://learn.microsoft.com/en-us/azure/azure-functions/configure-monitoring?tabs=v2
    /*
     *  Host.json path	App setting
        logging.logLevel.default	                AzureFunctionsJobHost__logging__logLevel__default
        logging.logLevel.Host.Aggregator	        AzureFunctionsJobHost__logging__logLevel__Host.Aggregator
        logging.logLevel.Function	                AzureFunctionsJobHost__logging__logLevel__Function
        logging.logLevel.Function.Function1	        AzureFunctionsJobHost__logging__logLevel__Function.Function1
        logging.logLevel.Function.Function1.User	AzureFunctionsJobHost__logging__logLevel__Function.Function1.User
        
    "code"-Redirect problem: https://www.how2code.info/en/blog/solving-azure-static-web-apps-problem-with-code-query-parameter/
     */
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = builder.GetContext().Configuration;
        // builder.Services.AddTransient(provider => new OidcSettings(config));
        // var connectionString = config.GetValue<string>("BEXIOSTOR");
        // var connString = new AzureStorageConnectionString(connectionString);
        // builder.Services.AddSingleton(connString);
        // builder.Services.AddTransient<SwaIdentity>();
        // builder.Services.AddTransient<TableStorageService>();
        // builder.Services.AddTransient<AuthenticationService>();
    }
}
using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Storage.Extensions;
using affolterNET.TextExtractor.Web.Services;
using Serilog;
using affolterNET.Web.Bff.Extensions;
using affolterNET.Web.Core.Models;
using affolterNET.Web.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add local secrets for environment-specific configuration
builder.Configuration.AddLocalSecrets(
    builder.Environment.EnvironmentName,
    "affolterNET.TextExtractor.json"
);

var isDev = builder.Environment.IsDevelopment();
var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Configure Serilog
if (isDev && !isRunningInContainer)
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
}
else
{
    // Azure Container Apps - output to stdout for Azure Monitor
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
}

builder.Host.UseSerilog();

// Configure Kestrel for Azure Container Apps
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;

    if (isRunningInContainer)
    {
        var port = Environment.GetEnvironmentVariable("PORT");
        if (int.TryParse(port, out var portNumber))
        {
            serverOptions.ListenAnyIP(portNumber);
        }
        else
        {
            serverOptions.ListenAnyIP(8080);
        }
    }
});

// Add MVC services
builder.Services.AddControllersWithViews();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services.AddRazorPages();

// Configure BFF authentication - using None mode for testing (no auth)
// TODO: Switch to AuthenticationMode.Authenticate when Keycloak is configured
var appSettings = new AppSettings(isDev, AuthenticationMode.None, true);
var bffOptions = builder.Services.AddBffServices(appSettings, builder.Configuration, options =>
{
    options.ConfigureBff = bffOptions =>
    {
        bffOptions.EnableHttpsRedirection = !isRunningInContainer;
    };
});

Log.Logger.Information("Bff Configuration: {0}", bffOptions.ToJson());
bffOptions.ValidateConfiguration();

// Register IOutput for TextExtractor pipeline logging
builder.Services.AddTransient<IOutput, LoggerOutput>();

// Add TextExtractor services
builder.Services.AddTextExtractorCoreServices(builder.Configuration);
builder.Services.AddTextExtractorStorageServices(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

var compileMode = "RELEASE";
#if DEBUG
compileMode = "DEBUG";
#endif

var env = isDev ? "DEVELOPMENT" : "PRODUCTION";
Log.Logger.Warning(
    "Runtime Environment - Docker: {runningInDocker}; Environment: {env}; Compile-Mode: {compileMode}",
    isRunningInContainer, env, compileMode);

// Configure BFF middleware pipeline
app.ConfigureBffApp(bffOptions);

try
{
    Log.Information("Starting affolterNET.TextExtractor.Web");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

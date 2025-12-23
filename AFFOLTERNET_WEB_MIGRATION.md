# affolterNET.Web Migration Plan

## Overview

This document outlines the plan to migrate affolterNET.TextExtractor from Azure Functions to an ASP.NET Core web application using the **affolterNET.Web.Bff** pattern.

## Key Changes

1. **Consolidate backend** - Merge `api/` and `backend/` into single `backend/` folder
2. **Remove Azure Functions** - Replace with ASP.NET Core MVC Controllers
3. **Add affolterNET.Web.Bff** - BFF (Backend for Frontend) pattern with YARP reverse proxy
4. **Integrate frontend** - Build Vue app directly into backend's `wwwroot/` folder
5. **Add Dockerfile** - Single container deployment with both frontend and backend
6. **Upgrade to .NET 9** - Use latest .NET version
7. **Authentication** - Start with Development mode (no auth server needed)

## Target Project Structure

```
affolterNET.TextExtractor/
├── frontend/                          # Vue 3 frontend
│   ├── src/
│   ├── vite.config.ts                # Build output: ../backend/wwwroot
│   └── package.json
├── backend/                           # Consolidated backend
│   ├── affolterNET.TextExtractor.Core/      # Business logic (existing)
│   ├── affolterNET.TextExtractor.Storage/   # Storage layer (existing)
│   ├── affolterNET.TextExtractor.Terminal/  # CLI tool (existing)
│   ├── affolterNET.TextExtractor.Web/       # NEW: Web + BFF
│   │   ├── Controllers/
│   │   ├── wwwroot/                  # Built frontend (gitignored)
│   │   ├── .secrets/                 # Local secrets (gitignored)
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── affolterNET.TextExtractor.Web.csproj
│   ├── Dockerfile                    # Builds both frontend and backend
│   └── .dockerignore
├── tests/                             # Tests (existing)
├── scripts/
│   └── docker/
│       └── docker-build-run.sh       # Build automation
└── affolterNET.TextExtractor.sln
```

## Migration Plan

### Phase 1: Create New Web Project

**Create** `backend/affolterNET.TextExtractor.Web/affolterNET.TextExtractor.Web.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
    <Version>0.1.0</Version>
  </PropertyGroup>

  <ItemGroup>
    <!-- affolterNET.Web BFF package -->
    <PackageReference Include="affolterNET.Web.Bff" Version="0.3.13" />

    <!-- Logging and monitoring -->
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />

    <!-- OpenAPI/Swagger -->
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.9" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="9.0.4" />
  </ItemGroup>

  <ItemGroup>
    <!-- Existing TextExtractor libraries -->
    <ProjectReference Include="../affolterNET.TextExtractor.Core/affolterNET.TextExtractor.Core.csproj" />
    <ProjectReference Include="../affolterNET.TextExtractor.Storage/affolterNET.TextExtractor.Storage.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="wwwroot\" />
    <Folder Include=".secrets\" />
    <Folder Include="logs\" />
  </ItemGroup>

  <ItemGroup>
    <!-- Copy secrets if they exist -->
    <Content Include=".secrets\affolterNET.TextExtractor.json" Condition="Exists('.secrets\affolterNET.TextExtractor.json')">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include=".secrets\affolterNET.TextExtractor.development.json" Condition="Exists('.secrets\affolterNET.TextExtractor.development.json')">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include=".secrets\affolterNET.TextExtractor.production.json" Condition="Exists('.secrets\affolterNET.TextExtractor.production.json')">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
</Project>
```

**Create** `backend/affolterNET.TextExtractor.Web/Program.cs`:

```csharp
using affolterNET.TextExtractor.Core.Extensions;
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

// Configure BFF authentication - start with Development mode
var appSettings = new AppSettings(isDev, AuthenticationMode.Development, true);
var bffOptions = builder.Services.AddBffServices(appSettings, builder.Configuration, options =>
{
    options.ConfigureBff = bffOptions =>
    {
        bffOptions.EnableHttpsRedirection = !isRunningInContainer;
    };
});

Log.Logger.Information("Bff Configuration: {0}", bffOptions.ToJson());
bffOptions.ValidateConfiguration();

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
```

**Create** `backend/affolterNET.TextExtractor.Web/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Create** `backend/affolterNET.TextExtractor.Web/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### Phase 2: Migrate Azure Functions to Controllers

**Create** `backend/affolterNET.TextExtractor.Web/Controllers/InfoController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace affolterNET.TextExtractor.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly ILogger<InfoController> _logger;

    public InfoController(ILogger<InfoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Info endpoint called");
        return Ok(new
        {
            status = "ok",
            service = "affolterNET.TextExtractor",
            version = "0.1.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        });
    }
}
```

**Create other controllers as needed**:
- `UploadController` - File upload (when re-enabled)
- `ExtractController` - Text extraction API

### Phase 3: Configure Frontend Integration

**Update** `frontend/vite.config.ts`:

```typescript
import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    port: 5173,
    strictPort: true,
    host: 'localhost',
    // Proxy API calls to backend during development
    proxy: {
      '/api': {
        target: 'https://localhost:7071',
        changeOrigin: true,
        secure: false
      },
      '/bff': {
        target: 'https://localhost:7071',
        changeOrigin: true,
        secure: false
      }
    },
    hmr: {
      protocol: 'wss',
      overlay: true
    }
  },
  build: {
    // Output directly to backend wwwroot
    outDir: '../backend/affolterNET.TextExtractor.Web/wwwroot',
    emptyOutDir: true,
    rollupOptions: {
      output: {
        entryFileNames: 'js/[name]-[hash].js',
        chunkFileNames: 'js/[name]-[hash].js',
        assetFileNames: (assetInfo) => {
          if (/\.(css)$/.test(assetInfo.name || '')) {
            return 'css/[name]-[hash].[ext]';
          }
          if (/\.(png|jpe?g|svg|gif|tiff|bmp|ico)$/i.test(assetInfo.name || '')) {
            return 'images/[name]-[hash].[ext]';
          }
          return 'assets/[name]-[hash].[ext]';
        }
      }
    }
  }
})
```

**Update** `frontend/package.json` - add scripts and dependency:

```json
{
  "scripts": {
    "dev": "vite --host 0.0.0.0",
    "start": "concurrently \"npm run dev\" \"npm run start:api\"",
    "start:api": "cd ../backend/affolterNET.TextExtractor.Web && dotnet run",
    "build": "run-p type-check build-only",
    "build-only": "vite build",
    "preview": "vite preview",
    "type-check": "vue-tsc --build --force",
    "lint": "eslint . --ext .vue,.js,.jsx,.cjs,.mjs,.ts,.tsx,.cts,.mts --fix --ignore-path .gitignore",
    "format": "prettier --write src/",
    "test:unit": "vitest",
    "test:e2e": "start-server-and-test preview http://localhost:4173 'cypress run --e2e'",
    "test:e2e:dev": "start-server-and-test 'vite dev --port 4173' http://localhost:4173 'cypress open --e2e'"
  },
  "devDependencies": {
    "concurrently": "^8.2.2"
    // ... existing devDependencies
  }
}
```

**Add to** `.gitignore`:

```
# Frontend build output (generated by npm build)
backend/affolterNET.TextExtractor.Web/wwwroot/
backend/affolterNET.TextExtractor.Web/logs/
```

### Phase 4: Create Dockerfile

**Create** `backend/Dockerfile`:

```dockerfile
# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base

# Create non-root user for security
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -u 1001 -G appgroup

WORKDIR /app

# Azure Container Apps configuration
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Build argument for version
ARG CONTAINER_IMAGE_VERSION=unknown
ENV CONTAINER_IMAGE_VERSION=${CONTAINER_IMAGE_VERSION}

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

RUN apk add --no-cache git curl

WORKDIR /src

# Copy all project files for restore
COPY ["affolterNET.TextExtractor.Web/affolterNET.TextExtractor.Web.csproj", "affolterNET.TextExtractor.Web/"]
COPY ["affolterNET.TextExtractor.Core/affolterNET.TextExtractor.Core.csproj", "affolterNET.TextExtractor.Core/"]
COPY ["affolterNET.TextExtractor.Storage/affolterNET.TextExtractor.Storage.csproj", "affolterNET.TextExtractor.Storage/"]

# Restore dependencies
RUN dotnet restore "affolterNET.TextExtractor.Web/affolterNET.TextExtractor.Web.csproj" --disable-parallel

# Copy the entire backend source code
COPY . .

# Remove secrets for security
RUN rm -rf .secrets || true

# Build the application
WORKDIR "/src/affolterNET.TextExtractor.Web"
RUN dotnet build "affolterNET.TextExtractor.Web.csproj" -c Release --no-restore

# Publish stage
FROM build AS publish
WORKDIR "/src/affolterNET.TextExtractor.Web"

RUN dotnet publish "affolterNET.TextExtractor.Web.csproj" -c Release -o /app/publish \
    --no-restore \
    --no-build \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false \
    /p:PublishReadyToRun=false

# Final stage
FROM base AS final

# Switch to non-root user
USER appuser

# Copy published application
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "affolterNET.TextExtractor.Web.dll"]
```

**Create** `backend/.dockerignore`:

```
# .NET build outputs
**/bin/
**/obj/
**/out/

# Logs
logs/
*.log

# Secrets
.secrets/
**/.secrets/

# Environment files
.env
.env.local
.env.development
.env.production

# IDE files
.vs/
.vscode/
.idea/
*.user
*.suo

# Git
.git/
.gitignore
README.md
*.md

# Tests
**/tests/

# Other projects (not needed for build)
affolterNET.TextExtractor.Terminal/
```

### Phase 5: Create Build Script

**Create** `scripts/docker/docker-build-run.sh`:

```bash
#!/bin/bash

set -e

# Configuration
CONTAINER_NAME="affolternet-textextractor-web"
IMAGE_NAME="affolternet-textextractor-web"
IMAGE_TAG="0.1.0"
PORT="8080"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

# Get script directory and project root
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
PROJECT_ROOT="$(dirname "$(dirname "$SCRIPT_DIR")")"
FRONTEND_DIR="$PROJECT_ROOT/frontend"
BACKEND_DIR="$PROJECT_ROOT/backend"

echo -e "${BLUE}[INFO]${NC} Building frontend application..."
cd "$FRONTEND_DIR"

if [ ! -f "package.json" ]; then
    echo -e "${RED}[ERROR]${NC} package.json not found in frontend directory"
    exit 1
fi

echo -e "${BLUE}[INFO]${NC} Installing frontend dependencies..."
npm install

echo -e "${BLUE}[INFO]${NC} Building frontend application..."
npm run build

if [ $? -ne 0 ]; then
    echo -e "${RED}[ERROR]${NC} Frontend build failed"
    exit 1
fi

echo -e "${GREEN}[SUCCESS]${NC} Frontend built to backend/affolterNET.TextExtractor.Web/wwwroot"

echo -e "${BLUE}[INFO]${NC} Building Docker image..."
cd "$BACKEND_DIR"

docker build -t "$IMAGE_NAME:$IMAGE_TAG" .

if [ $? -ne 0 ]; then
    echo -e "${RED}[ERROR]${NC} Docker build failed"
    exit 1
fi

echo -e "${GREEN}[SUCCESS]${NC} Docker image built: $IMAGE_NAME:$IMAGE_TAG"

# Stop existing container if running
if docker ps -q -f name="$CONTAINER_NAME" | grep -q .; then
    echo -e "${BLUE}[INFO]${NC} Stopping existing container..."
    docker stop "$CONTAINER_NAME"
    docker rm "$CONTAINER_NAME"
fi

echo -e "${BLUE}[INFO]${NC} Starting new container: $CONTAINER_NAME"
docker run -d \
    --name "$CONTAINER_NAME" \
    -p "$PORT:8080" \
    --restart unless-stopped \
    -e "CONTAINER_IMAGE_VERSION=$IMAGE_TAG" \
    "$IMAGE_NAME:$IMAGE_TAG"

echo -e "${GREEN}[SUCCESS]${NC} Container started successfully!"
echo -e "${BLUE}[INFO]${NC} Access URL: http://localhost:$PORT"
```

Make executable:
```bash
chmod +x scripts/docker/docker-build-run.sh
```

### Phase 6: Update Solution and Remove Old Code

1. **Add new Web project to solution**:
   ```bash
   dotnet sln add backend/affolterNET.TextExtractor.Web/affolterNET.TextExtractor.Web.csproj
   ```

2. **Remove old API project**:
   ```bash
   dotnet sln remove api/api.csproj
   ```

3. **Delete old API directory**:
   ```bash
   rm -rf api/
   ```

4. **Test build**:
   ```bash
   dotnet build
   ```

### Phase 7: Update CLAUDE.md

Replace:
```markdown
**api/** - Azure Functions HTTP endpoints
- `HttpTriggers/InfoTrigger.cs` - Info endpoint (currently active)
- Uses isolated worker model (.NET 8)
```

With:
```markdown
**backend/affolterNET.TextExtractor.Web/** - ASP.NET Core Web + BFF
- `Controllers/` - MVC API controllers
- `wwwroot/` - Built frontend (generated from frontend/dist)
- BFF (Backend for Frontend) pattern with YARP reverse proxy
- OpenID Connect authentication support (Development mode by default)
- Uses affolterNET.Web.Bff NuGet package (v0.3.13)
- .NET 9.0
- Azure Container Apps optimized
```

Add build commands:
```markdown
### Backend (.NET 9)

Run backend:
```bash
cd backend/affolterNET.TextExtractor.Web
dotnet run
```

Build Docker container (includes frontend):
```bash
./scripts/docker/docker-build-run.sh
```

Or manually:
```bash
cd frontend
npm run build
cd ../backend
docker build -t textextractor:latest .
```

### Full Stack Development

Start both frontend and backend:
```bash
cd frontend
npm run start
```

This runs:
- Frontend dev server: http://localhost:5173
- Backend API: https://localhost:7071
- API calls proxied from frontend to backend
```

## Development Workflow

### Local Development

**Option 1: Separate processes (recommended for development)**

Terminal 1 - Backend:
```bash
cd backend/affolterNET.TextExtractor.Web
dotnet run
```

Terminal 2 - Frontend:
```bash
cd frontend
npm run dev
```

**Option 2: Combined start**:
```bash
cd frontend
npm run start
```

### Production Build and Deploy

1. **Build frontend**:
   ```bash
   cd frontend
   npm run build
   ```

2. **Build and run Docker container**:
   ```bash
   ./scripts/docker/docker-build-run.sh
   ```

3. **Access application**:
   ```
   http://localhost:8080
   ```

## Authentication Configuration

### Development Mode (Default)
No configuration needed. Mock authentication is enabled.

### Production Mode (Keycloak/Azure AD)

Update Program.cs:
```csharp
var appSettings = new AppSettings(isDev, AuthenticationMode.Authenticate, true);
```

Add to `.secrets/affolterNET.TextExtractor.production.json`:
```json
{
  "affolterNET": {
    "Web": {
      "Auth": {
        "Provider": {
          "AuthorityBase": "https://your-keycloak.com",
          "Realm": "your-realm",
          "ClientId": "textextractor-client",
          "ClientSecret": "your-client-secret"
        }
      }
    }
  }
}
```

## Migration Checklist

- [ ] Create new Web project (Phase 1)
- [ ] Create InfoController (Phase 2)
- [ ] Test backend: `cd backend/affolterNET.TextExtractor.Web && dotnet run`
- [ ] Configure frontend build to wwwroot (Phase 3)
- [ ] Test frontend build: `cd frontend && npm run build`
- [ ] Create Dockerfile (Phase 4)
- [ ] Create build script (Phase 5)
- [ ] Test Docker build: `./scripts/docker/docker-build-run.sh`
- [ ] Update solution file (Phase 6)
- [ ] Migrate remaining controllers (Phase 2 continued)
- [ ] Delete old api/ directory
- [ ] Update CLAUDE.md (Phase 7)
- [ ] Test full workflow (dev and production)
- [ ] (Optional) Configure real authentication

## Benefits

1. **Unified Backend**: All backend code in one `backend/` folder
2. **Latest .NET**: Using .NET 9 with latest features and performance
3. **Single Container**: Frontend and backend deployed together
4. **Better Security**: BFF pattern, session cookies instead of bearer tokens
5. **Consistent Architecture**: Same pattern as affolterNET.Bexio
6. **Standard ASP.NET Core**: No Azure Functions lock-in
7. **Development Experience**: Hot reload for both frontend and backend
8. **Azure Container Apps Ready**: Optimized for modern Azure deployment

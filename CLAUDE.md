# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

affolterNET.TextExtractor is a PDF text extraction system that uses a pipeline architecture to extract, analyze, and structure text from PDF documents. The system is deployed as Azure Functions.

**Key components:**
- **Backend (.NET 8)**: Core text extraction engine with pipeline-based processing
- **API (Azure Functions)**: HTTP triggers for document upload and processing
- **Frontend (Vue 3 + Vite)**: Web UI for document upload and management

## Build and Run Commands

### Backend (.NET)

Build the solution:
```bash
dotnet build affolterNET.TextExtractor.sln
```

Run tests:
```bash
dotnet test tests/affolterNET.TextExtractor.Core.Test/affolterNET.TextExtractor.Core.Test.csproj
```

Run terminal application:
```bash
cd backend/affolterNET.TextExtractor.Terminal
dotnet run -- parse-pdf -f <PATH_TO_PDF>
```

Run Azure Functions locally:
```bash
cd api
func start
# or
dotnet run
```

### Frontend (Vue 3)

Install dependencies:
```bash
cd frontend
npm install
```

Development server:
```bash
npm run dev
```

Build for production:
```bash
npm run build
```

Type checking:
```bash
npm run type-check
```

Run unit tests:
```bash
npm run test:unit
```

Run E2E tests:
```bash
npm run test:e2e:dev  # Development mode
npm run test:e2e      # Production mode
```

Lint code:
```bash
npm run lint
```

Run with Azure Static Web Apps CLI:
```bash
npm run start  # Uses local API
npm run debug  # Uses API at localhost:7071
```

## Architecture

### Processing Pipeline

The core text extraction uses a sequential pipeline pattern where each step processes PDF pages and passes context to the next step. Pipeline is defined in `backend/affolterNET.TextExtractor.Core/Pipeline/`.

**Pipeline architecture:**
1. **ProcessingPipeline**: Generic pipeline executor that runs steps sequentially
2. **BasicPdfPipeline**: Concrete implementation with PDF-specific steps
3. **IPipelineStep**: Interface all processing steps implement
4. **IPipelineContext**: Shared context passed through the pipeline

**Standard processing steps (in order):**
1. `ReadPagesStep` - Reads PDF pages using PdfPig library
2. `CleanWordsStep` - Cleans and normalizes word data
3. `DetectTextBlocksStep` - Groups words into text blocks
4. `AnalyzeLineSpacingStep` - Analyzes spacing between lines
5. `DetectPageNumberStep` - Identifies page numbers
6. `DetectHeadersStep` - Identifies headers
7. `DetectFootnotesStep` - Identifies footnotes
8. `FixSpacesStep` - Corrects spacing issues
9. `ExtractTextStep` - Final text extraction (currently commented out)

**Adding new pipeline steps:**
- Implement `IPipelineStep` interface
- Add step to DI container in `ConfigureServicesExtension.cs`
- Insert step in desired order in `BasicPdfPipeline` constructor

### Project Structure

**backend/affolterNET.TextExtractor.Core** - Core extraction logic
- `Models/` - Domain models (PdfPage, PdfTextBlock, PdfDoc, etc.)
- `Pipeline/` - Pipeline infrastructure and steps
- `Services/Detectors/` - Detection services (headers, footnotes, blocks, etc.)
- `Extensions/` - Extension methods and DI configuration

**backend/affolterNET.TextExtractor.Storage** - Azure Blob Storage integration
- Handles persistence of extracted JSON to blob storage
- Configured via `AddTextExtractorStorageServices()` extension

**backend/affolterNET.TextExtractor.Terminal** - CLI application
- Uses Spectre.Console for terminal UI
- Main command: `parse-pdf` to process PDF files
- Outputs results to file system

**api/** - Azure Functions HTTP endpoints
- `HttpTriggers/InfoTrigger.cs` - Info endpoint (currently active)
- `HttpTriggers/UploadTrigger.cs` - File upload (disabled)
- `HttpTriggers/BlobTrigger.cs` - Blob processing (disabled)
- Uses isolated worker model (.NET 8)
- ApplicationInsights integration for monitoring

**frontend/** - Vue 3 SPA
- Vue 3 with TypeScript
- Vite for build tooling
- Pinia for state management
- Vue Router for routing
- Uses affolternet-vue3-library and beercss
- Configured for Azure Static Web Apps deployment

### Dependency Injection

Both Terminal and API applications use Microsoft.Extensions.DependencyInjection:

**Core services registration:**
```csharp
services.AddTextExtractorCoreServices(configuration);
```
Registers all pipeline steps, detectors, and core services.

**Storage services registration:**
```csharp
services.AddTextExtractorStorageServices(configuration);
```
Registers blob storage services.

**Logging abstraction:**
The `IOutput` interface abstracts logging:
- `FunctionsLogger` - Used in Azure Functions
- `FileOutputter` - Used in Terminal app
- Supports custom log levels via `EnumLogLevel`

### Configuration

**API (Azure Functions):**
- `local.settings.json` - Local development settings
- `appsettings.json` - Application configuration
- User Secrets for sensitive data (configured in csproj)
- Environment variables in Azure

**Terminal:**
- User Secrets for local development
- Command line arguments (see Program.cs for mappings)
- Environment variables
- `appsettings.json`

**Frontend:**
- Environment-specific configuration via Vite
- Static Web Apps CLI configuration in package.json

### Testing

Test project uses xUnit framework. Tests are in `tests/affolterNET.TextExtractor.Core.Test/`.

Current test setup:
- xUnit 2.6.6
- Microsoft.NET.Test.Sdk
- Code coverage via coverlet.collector

### Key Dependencies

**Backend:**
- PdfPig 0.1.8 - PDF parsing library
- Azure.Storage.Blobs - Blob storage
- Microsoft.Azure.Functions.Worker - Azure Functions isolated worker
- Spectre.Console - Terminal UI (Terminal app only)

**Frontend:**
- Vue 3.3.11
- Vite 5.0.10
- TypeScript 5.3
- Pinia 2.1.7 - State management
- Cypress 13.6.1 - E2E testing
- Vitest 1.0.4 - Unit testing

## Development Notes

**Current state:**
- Most Azure Functions are disabled (only InfoTrigger is active)
- ExtractTextStep is commented out in BasicPdfPipeline (line 30)
- Git workflow uses `main` branch for PRs, currently on `develop` branch

**When modifying pipelines:**
- Pipeline steps execute sequentially and share context
- Each step should be idempotent where possible
- Context object (`IPipelineContext`) carries all state between steps
- Register new services in `ConfigureServicesExtension.cs`

**When working with PDFs:**
- PdfPig library is used for low-level PDF access
- Custom models wrap PdfPig types (see Models/Interfaces/)
- Spatial analysis uses custom quadtree implementation
- Text extraction considers font sizes, spacing, and positioning

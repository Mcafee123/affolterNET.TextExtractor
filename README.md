# affolterNET.TextExtractor

PDF text extraction service with Vue 3 frontend and ASP.NET Core backend using the BFF (Backend for Frontend) pattern.

## Architecture

- **Frontend**: Vue 3 + TypeScript + Vite
- **Backend**: ASP.NET Core 9.0 Web + YARP Reverse Proxy
- **Pattern**: Backend for Frontend (BFF) with YARP
- **Auth**: Keycloak (OpenID Connect)

## Development Setup

### Prerequisites

- .NET 9.0 SDK
- Node.js 22.12.0+ (for Vite 7)
- JetBrains Rider or Visual Studio (recommended)

### SSL Certificates

Both frontend and backend use HTTPS in development:

**Frontend (mkcert - auto-trusted)**:
```bash
cd frontend
mkcert -install  # First time only
cd certs
mkcert -key-file dev-cert.key -cert-file dev-cert.pem localhost 127.0.0.1 ::1
```

**Backend (.NET dev certs)**:
```bash
dotnet dev-certs https --trust
```

### Running the Application

The application uses **YARP reverse proxy** in the ASP.NET Core Web project to route requests between frontend and backend.

#### Step 1: Start Vite Dev Server (Terminal 1)

```bash
cd frontend
npm install  # First time only
npm run dev  # Runs on https://localhost:5173
```

This provides:
- Hot Module Replacement (HMR)
- Fast refresh during development
- Source maps for debugging

#### Step 2: Start ASP.NET Core Backend (Rider)

1. Open solution in Rider
2. Select `affolterNET.TextExtractor.Web` project
3. Run configuration: Select **"https"** profile
4. Click Run (⌃R)
5. Backend starts on `https://localhost:5000`

#### Step 3: Access the Application

**🌐 Open: https://localhost:5000**

**Important**: Access the app through the backend URL (port 5000), NOT the Vite URL (port 5173)!

### How YARP Routing Works

When you access `https://localhost:5000`:

```
Request → ASP.NET Core Web (port 5000) → YARP Reverse Proxy
                    │
                    ├─→ /api/*           → Backend Controllers
                    ├─→ Frontend files   → Vite Dev Server (port 5173)
                    └─→ WebSocket/HMR    → Vite Dev Server (port 5173)
```

**Benefits**:
- Single origin (no CORS issues)
- Unified authentication
- API and frontend on same domain
- Matches production setup

### Frontend Development

```bash
cd frontend

# Install dependencies
npm install

# Run dev server (must run alongside backend)
npm run dev

# Type checking
npm run type-check

# Linting (ESLint 9 with flat config)
npm run lint

# Build for production
npm run build  # Outputs to ../backend/affolterNET.TextExtractor.Web/wwwroot
```

### Backend Development

```bash
cd backend/affolterNET.TextExtractor.Web

# Restore packages
dotnet restore

# Run with HTTPS
dotnet run --launch-profile https

# Build
dotnet build

# Run tests
cd ../../tests/affolterNET.TextExtractor.Core.Test
dotnet test
```

### Configuration

**Backend**: `backend/affolterNET.TextExtractor.Web/appsettings.json`
- BFF URLs (frontend/backend)
- Keycloak settings
- Storage account settings

**Frontend**: `frontend/vite.config.ts`
- Build output directory
- HTTPS certificates
- HMR configuration

**Launch Profiles**: `backend/affolterNET.TextExtractor.Web/Properties/launchSettings.json`
- https profile (default)
- http profile

### Troubleshooting

**"JSON parse error" in browser**:
- Make sure backend is running on https://localhost:5000
- Access app through https://localhost:5000 (not :5173)
- Check backend logs for errors

**Certificate warnings**:
- Frontend: Run `mkcert -install` to trust certificates
- Backend: Run `dotnet dev-certs https --trust`

**Port already in use**:
- Frontend: Kill process on port 5173: `lsof -ti:5173 | xargs kill`
- Backend: Kill process on port 5000: `lsof -ti:5000 | xargs kill`

**HMR not working**:
- Make sure you're accessing through https://localhost:5000
- Check YARP is correctly proxying WebSocket connections
- Restart both Vite and backend servers

## Project Structure

```
affolterNET.TextExtractor/
├── backend/
│   ├── affolterNET.TextExtractor.Core/      # Core business logic
│   ├── affolterNET.TextExtractor.Storage/   # Azure Blob Storage
│   ├── affolterNET.TextExtractor.Terminal/  # CLI tool
│   └── affolterNET.TextExtractor.Web/       # ASP.NET Core Web + BFF
│       ├── Controllers/                      # API endpoints
│       ├── Properties/launchSettings.json   # Launch profiles
│       └── wwwroot/                          # Built frontend (production)
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── views/
│   │   └── main.ts
│   ├── certs/                                # mkcert SSL certificates (gitignored)
│   ├── eslint.config.mts                    # ESLint 9 flat config
│   ├── vite.config.ts                       # Vite configuration
│   └── package.json
├── tests/
│   └── affolterNET.TextExtractor.Core.Test/
└── tf/                                       # Terraform infrastructure
```

## Technology Stack

### Frontend
- Vue 3.5.26
- TypeScript 5.9.3
- Vite 7.3.0
- Vue Router 4.6.4
- Pinia 3.0.4 (state management)
- ESLint 9.39.2 (flat config)
- Vitest 4.0.16
- Cypress 15.8.1

### Backend
- .NET 9.0
- ASP.NET Core Web
- YARP (Reverse Proxy)
- Keycloak (Authentication)
- Azure Blob Storage
- Serilog (Logging)
- PdfPig 0.1.8 (PDF processing)

## See Also

- [AFFOLTERNET_WEB_MIGRATION.md](./AFFOLTERNET_WEB_MIGRATION.md) - Migration from Azure Functions to BFF pattern
- [Frontend README](./frontend/README.md) - Vue/Vite specific documentation

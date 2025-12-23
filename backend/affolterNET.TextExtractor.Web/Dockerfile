# Stage 1: Build frontend
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend

# Copy frontend package files
COPY frontend/package*.json ./
RUN npm ci

# Copy frontend source
COPY frontend/ ./

# Build frontend (outputs to ../backend/affolterNET.TextExtractor.Web/wwwroot)
RUN npm run build

# Stage 2: Build backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /app

# Copy solution and project files
COPY affolterNET.TextExtractor.sln ./
COPY backend/affolterNET.TextExtractor.Core/affolterNET.TextExtractor.Core.csproj ./backend/affolterNET.TextExtractor.Core/
COPY backend/affolterNET.TextExtractor.Storage/affolterNET.TextExtractor.Storage.csproj ./backend/affolterNET.TextExtractor.Storage/
COPY backend/affolterNET.TextExtractor.Web/affolterNET.TextExtractor.Web.csproj ./backend/affolterNET.TextExtractor.Web/

# Restore dependencies
RUN dotnet restore backend/affolterNET.TextExtractor.Web/affolterNET.TextExtractor.Web.csproj

# Copy source code
COPY backend/ ./backend/

# Copy frontend build output to wwwroot
COPY --from=frontend-build /app/backend/affolterNET.TextExtractor.Web/wwwroot ./backend/affolterNET.TextExtractor.Web/wwwroot

# Build and publish
WORKDIR /app/backend/affolterNET.TextExtractor.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install required packages for PDF processing
RUN apt-get update && apt-get install -y \
    libgdiplus \
    && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=backend-build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "affolterNET.TextExtractor.Web.dll"]

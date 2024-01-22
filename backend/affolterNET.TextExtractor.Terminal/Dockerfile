FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["affolterNET.TextExtractor.Terminal/affolterNET.TextExtractor.Terminal.csproj", "affolterNET.TextExtractor.Terminal/"]
RUN dotnet restore "affolterNET.TextExtractor.Terminal/affolterNET.TextExtractor.Terminal.csproj"
COPY . .
WORKDIR "/src/affolterNET.TextExtractor.Terminal"
RUN dotnet build "affolterNET.TextExtractor.Terminal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "affolterNET.TextExtractor.Terminal.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "affolterNET.TextExtractor.Terminal.dll"]

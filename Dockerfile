# ── Build Stage ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY HrSystem.Backend.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Runtime Stage ────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser:appuser /app
USER appuser

# Copy published output & data folder for SQLite
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "HrSystem.Backend.dll"]

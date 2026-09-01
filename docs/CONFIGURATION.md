# Configuration & Secrets

This project keeps non-sensitive settings in `src/HrSystem.Api/appsettings.json`.

Sensitive values are not committed to Git.

## Local development

The API project has a `UserSecretsId`. Configure the local SQL Server connection string and JWT signing key with .NET User Secrets:

```bash
dotnet user-secrets --project src/HrSystem.Api set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=HrSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet user-secrets --project src/HrSystem.Api set "Jwt:Key" "<long-random-secret-at-least-32-characters>"
```

Redis is optional for local development. Without a Redis connection, token revocation uses the in-process distributed-memory cache.

To use Redis locally:

```bash
dotnet user-secrets --project src/HrSystem.Api set "ConnectionStrings:Redis" "localhost:6379"
```

## Production

Provide secrets through environment variables or a platform secret manager. ASP.NET Core maps `__` in environment variables to `:` in configuration keys.

```text
ConnectionStrings__DefaultConnection=<production-sql-server-connection-string>
ConnectionStrings__Redis=<redis-connection-string>
Jwt__Key=<long-random-secret-at-least-32-characters>
```

Do not put production credentials, passwords, API keys, Redis connection strings, or JWT signing keys in tracked `appsettings.json` files.

## Non-secret configuration

Issuer, audience, token lifetimes, CORS origins, and logging defaults remain in `appsettings.json` because they are application configuration rather than secrets.

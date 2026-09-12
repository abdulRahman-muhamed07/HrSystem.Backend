# Local development setup

The Development environment uses Visual Studio SQL Server LocalDB:

`Server=(localdb)\MSSQLLocalDB;Database=HrSystemDb;Trusted_Connection=True;TrustServerCertificate=True`

Make sure LocalDB is available:

```powershell
sqllocaldb info
sqllocaldb start MSSQLLocalDB
```

Then run:

```powershell
dotnet restore
dotnet build
dotnet run --project src/HrSystem.Api
```

The API creates `HrSystemDb` automatically in Development via EF Core `EnsureCreatedAsync()`.

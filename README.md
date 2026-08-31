# HR System Backend

A layered ASP.NET Core 10 Web API organized around Clean Architecture and Dependency Inversion.

## Architecture

```text
src/
  HrSystem.Domain/
    Entities/          # one entity per file
    Enums/
  HrSystem.Application/
    Exceptions/
    Services/          # use cases + contracts + abstractions
  HrSystem.Infrastructure/
    Persistence/       # EF Core DbContext + repository
    Security/          # JWT + password hashing
    Auditing/
  HrSystem.Api/
    Controllers/
    Middleware/
    Security/
```

Dependency direction:
`Api -> Infrastructure -> Application -> Domain`.
Domain has no infrastructure dependency. Application depends on abstractions, while Infrastructure supplies their implementations.

## Main capabilities

Authentication with JWT and role-based authorization (`Admin`, `HR`, `Employee`), employee management with search/pagination, departments, attendance check-in/check-out, leave requests and balance validation, overtime, employee loans, payroll generation/payment, dashboard summary, and audit logs.

## Database

EF Core lives in `HrSystem.Infrastructure/Persistence`. The default development database is SQLite. SQL Server can be selected with `Database:Provider = sqlserver` and the configured connection string.

Local development uses `EnsureCreated` so a fresh clone can start quickly. For production, create and apply EF migrations from the Infrastructure project:

```bash
dotnet ef migrations add InitialCreate --project src/HrSystem.Infrastructure --startup-project src/HrSystem.Api
dotnet ef database update --project src/HrSystem.Infrastructure --startup-project src/HrSystem.Api
```

Do not commit real database files or secrets. Override `Jwt:Key` with .NET User Secrets or an environment variable in real environments.

## Run

```bash
dotnet restore HrSystem.Backend.slnx
dotnet run --project src/HrSystem.Api
```

Swagger is available in Development, and `/health` checks database connectivity.

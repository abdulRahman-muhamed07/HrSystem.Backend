# HR System Backend

Production-style HR management API built with ASP.NET Core and Clean Architecture.

## Architecture
- Domain: entities, enums, and business rules.
- Application: DTOs, service abstractions, persistence abstractions, security abstractions, and application services.
- Infrastructure: EF Core persistence, repositories, security, and auditing.
- API: controllers, middleware, authentication, rate limiting, and OpenAPI.

## Application Structure
Each DTO and abstraction is kept in its own file and grouped by responsibility:

```text
src/HrSystem.Application
├── DTOs
├── Abstractions
│   ├── Services
│   ├── Persistence
│   ├── Security
│   └── Auditing
└── Services
```

## Engineering Practices
- Dependency Inversion and separation of concerns.
- Thin controllers and application-level service abstractions.
- Centralized API error handling.
- JWT authentication and role-based authorization.
- PBKDF2 password hashing.
- Pagination/search and repository abstractions.
- EF Core configuration separated from the DbContext.
- Health endpoint, rate limiting, Docker, CI, and automated tests.

## Database
SQLite is convenient for local development; SQL Server is supported through configuration.
Never commit database files, secrets, or environment-specific credentials.

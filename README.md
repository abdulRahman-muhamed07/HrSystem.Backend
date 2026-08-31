# HR System Backend

Production-style HR management API built with ASP.NET Core and Clean Architecture.

## Architecture
- Domain: entities and business rules.
- Application: use cases, contracts, validation, and application services.
- Infrastructure: EF Core, repositories, security, auditing, and persistence.
- API: HTTP endpoints, middleware, authentication, rate limiting, and OpenAPI.

## Engineering Practices
Dependency Inversion, thin controllers, centralized error handling, JWT/RBAC, PBKDF2 password hashing, pagination/search, specialized repository contracts, EF configuration separation, health endpoint, CI, and dedicated test projects.

## Database
SQLite is used for local development. SQL Server is supported through configuration.
Never commit database files, secrets, or environment-specific credentials.

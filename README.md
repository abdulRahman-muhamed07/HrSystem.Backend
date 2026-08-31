# HR System Backend

A production-style HR management API built with ASP.NET Core and Clean Architecture.

## Architecture
- Domain: entities and business rules.
- Application: use cases, contracts, validation, and application services.
- Infrastructure: EF Core, repositories, security, auditing, and persistence.
- API: HTTP endpoints, middleware, authentication, rate limiting, and OpenAPI.

## Engineering
Dependency Inversion, thin controllers, centralized errors, JWT/RBAC, PBKDF2 password hashing, pagination/search, specialized repositories, isolated EF configurations, health endpoint, CI, and test projects.

## Database
SQLite is intended for local development; SQL Server is supported through configuration.
Never commit database files, secrets, or environment-specific credentials.

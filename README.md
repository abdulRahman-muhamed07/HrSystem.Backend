# HR System Backend

Production-style HR management API built with ASP.NET Core and Clean Architecture.

## Architecture
- Domain: entities, enums, and business rules.
- Application: DTOs, service abstractions, persistence abstractions, security abstractions, and application services.
- Infrastructure: EF Core persistence, repositories, security, and auditing.
- API: controllers, middleware, authentication, rate limiting, and OpenAPI.

## Application Structure
Each DTO and abstraction is kept in its own file and grouped by responsibility.

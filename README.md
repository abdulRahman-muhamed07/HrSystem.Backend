# HR System Backend

ASP.NET Core HR management backend built around Clean Architecture with feature-oriented application code.

## Structure

```text
src/
├── HrSystem.Api/            # HTTP boundary, controllers, middleware, composition
├── HrSystem.Application/    # Features, use cases, contracts, validation, abstractions
├── HrSystem.Domain/         # Entities, enums, domain rules
└── HrSystem.Infrastructure/ # EF Core, SQL Server, repositories, security, external integrations

tests/
├── HrSystem.Application.Tests/
├── HrSystem.Domain.Tests/
└── HrSystem.Infrastructure.Tests/
```

## Main capabilities

- Employee and department management
- Attendance check-in/check-out and reporting
- Leave requests and leave balances
- Overtime and employee loans
- Payroll generation and payment tracking
- Dashboard and audit logging
- JWT authentication, refresh tokens, and token revocation
- Role-based authorization
- Optimistic-concurrency support
- Pagination, validation, centralized exception handling, and health checks

## Architecture

`API -> Application -> Domain`

`Infrastructure -> Application + Domain`

Application defines the contracts and abstractions used by the use cases. Infrastructure provides the technical implementations. Controllers remain focused on HTTP concerns.

## Persistence

EF Core with SQL Server is used for relational persistence. Repository abstractions exist where feature-specific data access is useful; the generic repository is kept small and is not used as a reason to wrap every EF operation.

## Development

Use the .NET 10 SDK and provide database/JWT configuration through local configuration or environment variables. Never commit secrets, credentials, or environment-specific databases.

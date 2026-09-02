# Architecture

This backend uses Clean Architecture with feature-oriented (vertical-slice) organization inside the Application layer.

```text
API -> Application -> Domain
Infrastructure -> Application + Domain
```

## Layers

- `HrSystem.Api` — HTTP boundary, controllers, middleware, authentication wiring, and composition root.
- `HrSystem.Application` — feature use cases, contracts, handlers/services, validation, mappings, and abstractions for persistence/security/auditing.
- `HrSystem.Domain` — entities, enums, and business rules that are independent of infrastructure concerns.
- `HrSystem.Infrastructure` — EF Core/SQL Server persistence, repository implementations, security implementations, auditing, and technical integrations.
- `tests/*` — application, domain, and infrastructure tests plus architecture tests.

## Feature organization

Application features keep their contracts close to the feature they serve. A feature may contain request/response contracts, a handler for orchestration, and a service for the feature's application logic. Shared concerns stay outside individual features.

## Design rules

The Domain project must not depend on Application, Infrastructure, API, EF Core, or other transport concerns. Application owns interfaces needed by its use cases; Infrastructure implements those interfaces. API should not contain business rules or persistence code.

Repositories are used when they provide a meaningful persistence boundary or feature-specific query. Do not introduce abstractions that only forward every EF Core method without adding value.

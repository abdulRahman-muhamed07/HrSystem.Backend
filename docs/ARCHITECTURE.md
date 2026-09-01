# Architecture

This backend follows Clean Architecture with a vertical-slice approach inside the Application layer.

- `src/*Api` — HTTP boundary, controllers, middleware, authentication wiring, and composition root.
- `src/*Application` — use cases, feature contracts, DTOs, validation, mappings, and persistence abstractions.
- `src/*Domain` — entities, value-independent domain rules, enums, and domain contracts.
- `src/*Infrastructure` — EF Core persistence, repository implementations, security implementations, and external integrations.
- `tests/*` — automated tests grouped by architectural concern.

Dependency direction remains inward: API -> Application -> Domain, while Infrastructure implements Application abstractions.

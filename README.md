# HR System Backend

Production-style HR management API built with ASP.NET Core and Clean Architecture.

Architecture: Domain, Application, Infrastructure, and API layers with Dependency Inversion.

Engineering practices include thin controllers, centralized errors, JWT/RBAC, PBKDF2 password hashing, pagination/search, repository abstractions, EF Core persistence separation, rate limiting support, health checks, CI, and tests.

SQLite is for local development; SQL Server is supported through configuration.
Never commit databases, secrets, or environment-specific credentials.

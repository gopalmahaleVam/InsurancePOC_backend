---
description: 'Guidelines for building C# applications in .NET Core greenfield projects'
applyTo: '**/*.cs'
---

# C# Development

## C# Instructions
- Always use the latest version C#, currently C# 14 features.
- Write clear and concise comments for each function explaining "why", not "what".

## General Instructions
- Make only high confidence suggestions when reviewing code changes.
- Write code with good maintainability practices, including comments on why certain design decisions were made.
- Handle edge cases and write clear exception handling.
- For libraries or external dependencies, mention their usage and purpose in comments.

## Naming Conventions

- Follow PascalCase for component names, method names, and public members.
- Use camelCase for private fields and local variables.
- Prefix interface names with "I" (e.g., IUserService).
- Suffix async methods with "Async" (e.g., GetCustomerAsync).
- Suffix Commands with "Command" and Queries with "Query".
- Suffix Handlers with "Handler".

## Formatting

- Apply code-formatting style defined in `.editorconfig`.
- Prefer file-scoped namespace declarations and single-line using directives.
- Insert a newline before the opening curly brace of any code block.
- Ensure that the final return statement of a method is on its own line.
- Use pattern matching and switch expressions wherever possible.
- Use `nameof` instead of string literals when referring to member names.
- Ensure that XML doc comments are created for any public APIs. When applicable, include `<example>` and `<code>` documentation in the comments.

## Project Setup and Structure

- Follow Clean Architecture with Domain, Application, Infrastructure, and API layers.
- Domain layer has zero external dependencies.
- Application layer depends only on Domain.
- Infrastructure layer implements interfaces defined in Domain/Application.
- API layer wires everything together via DI.
- Organize code using feature folders or domain-driven design principles.
- Explain the Program.cs and configuration system in ASP.NET Core including environment-specific settings.

## Nullable Reference Types

- Declare variables non-nullable, and check for `null` at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

## Data Access Patterns

- Use Entity Framework Core for data access.
- Implement repository pattern with interfaces in Domain and implementations in Infrastructure.
- Use async EF Core methods: `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`.
- Use `.AsNoTracking()` for read-only queries.
- Avoid N+1 queries — use `.Include()` or explicit loading.
- Use Code-First migrations for schema management.

## CQRS Patterns

- Use MediatR for command/query dispatching.
- Commands mutate state; Queries return data — never mix.
- Use pipeline behaviors for cross-cutting concerns (validation, logging, transactions).
- Integrate FluentValidation with MediatR pipeline via `ValidationBehavior`.

## Authentication and Authorization

- Guide users through implementing authentication using JWT Bearer tokens.
- Show how to implement role-based and policy-based authorization.
- Use `[Authorize]` attribute on controllers/actions.
- Hash passwords with BCrypt or Identity's built-in hasher.
- Never store passwords in plain text.

## Validation and Error Handling

- Use FluentValidation for Commands and Requests.
- Validate in handler/service layer before database operations.
- Implement global exception middleware for consistent error responses.
- Use Problem Details (RFC 9457) for standardized error responses.
- Use specific exception types — never throw or catch base `Exception`.

## API Versioning and Documentation

- Enable Swagger/OpenAPI with Swashbuckle.
- Document all endpoints, parameters, responses, and authentication requirements.
- Version APIs using URL versioning (`/api/v1/`) or header versioning.

## Logging and Monitoring

- Use `ILogger<T>` via dependency injection.
- Use Serilog for structured logging with enrichers (correlation ID, environment).
- Log levels: Trace, Debug, Information, Warning, Error, Critical.
- Don't log sensitive data (passwords, tokens, PII).
- Implement health checks (`/health`, `/ready`).
- Add Application Insights or OpenTelemetry for telemetry.

## Testing

- Always include test cases for critical paths of the application.
- Use xUnit as the default test framework.
- Do not emit "Act", "Arrange" or "Assert" comments.
- Follow AAA pattern: Arrange, Act, Assert.
- One behavior per test method.
- Name tests by behavior: `WhenCustomerNotFoundThenThrowsException`.
- Mock external dependencies only using Moq or NSubstitute.
- Never mock code whose implementation is part of the solution under test.

## Performance Optimization

- Implement caching strategies (in-memory, distributed, response caching).
- Use async/await end-to-end — no sync-over-async.
- Implement pagination, filtering, and sorting for large data sets.
- Use `Span<T>`, `Memory<T>`, pooling for hot paths when measured.

## Deployment and DevOps

- Containerize using .NET's built-in container support or Dockerfile.
- Use multi-stage Docker builds for smaller images.
- Use environment-specific `appsettings.{Environment}.json`.
- Use environment variables for secrets — never hardcode.
- Implement health checks for orchestrators (Kubernetes, Azure Container Apps).
- Follow 12-factor app principles.

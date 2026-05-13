---
description: 'Security, performance, and architecture guardrails for .NET Core greenfield development'
applyTo: '**/*.cs'
---

# .NET Core Development Guardrails

## Security Guardrails
- Do not hardcode connection strings, API keys, or secrets
- Do not use string concatenation for SQL queries
- Do not expose internal implementation details in API responses
- Do not log sensitive information (passwords, tokens, PII)
- Always validate and sanitize user inputs
- Always use HTTPS in production environments
- Always implement proper authentication and authorization
- Always use parameterized queries or ORM (EF Core)
- Always hash passwords — never store in plain text
- Always use environment variables or Azure Key Vault for secrets

## Performance Guardrails
- Do not use synchronous I/O operations in async contexts
- Do not create unnecessary database round trips (N+1 problem)
- Do not load entire collections when pagination is needed
- Do not ignore memory management for large datasets
- Always use async/await for I/O operations
- Always implement proper caching strategies
- Always use connection pooling for database connections
- Always dispose of resources properly (using statements)
- Always use `.AsNoTracking()` for read-only EF Core queries

## Code Quality Guardrails
- Do not use magic numbers or strings
- Do not create god classes or methods
- Do not ignore exception handling
- Do not skip unit tests for business logic
- Do not put business logic in controllers
- Always follow SOLID principles
- Always use meaningful variable and method names
- Always implement proper logging
- Always handle null reference scenarios
- Always use cancellation tokens for async operations

## Architecture Guardrails
- Do not reference higher layers from lower layers (Domain must have zero external dependencies)
- Do not put business logic in controllers — use handlers/services
- Do not create circular dependencies between projects
- Do not bypass the repository pattern for data access
- Do not expose domain entities directly through API responses — use DTOs
- Always maintain clear separation of concerns
- Always use dependency injection for service resolution
- Always implement interfaces for testability
- Always follow Clean Architecture principles
- Always use MediatR for command/query dispatching in the API layer

## Testing Guardrails
- Do not skip tests for critical business logic paths
- Do not mock code that is part of the solution under test
- Do not use `.Result` or `.Wait()` in test code — use async test methods
- Always write tests before or alongside feature implementation
- Always test edge cases and failure scenarios
- Always use meaningful test names that describe behavior

---
description: 'General .NET Core architecture and coding guidelines for greenfield development'
applyTo: '**/*.cs, **/*.csproj'
---

# .NET Core Development

## Core Principles
- Use C# 12+ and .NET 8 features
- Follow Clean Architecture principles with clear layer separation
- Implement SOLID principles throughout the codebase
- Use dependency injection for all service registrations
- Apply async/await patterns for I/O operations
- Implement proper error handling and logging
- Use strongly typed configurations
- Follow consistent naming conventions (PascalCase for public members, camelCase for private)

## Architecture Guidelines
- Domain layer: Pure business logic, no external dependencies
- Application layer: Use cases, commands, queries, handlers (CQRS with MediatR)
- Infrastructure layer: Data access, external services, implementations
- API layer: Controllers, middleware, filters, minimal APIs

## Clean Architecture Rules
- Dependencies flow inward only — outer layers depend on inner layers, never the reverse
- Domain layer has no references to any other project
- Application layer references only Domain
- Infrastructure layer references Application and Domain
- API layer references all layers for wiring via DI

## Code Quality Standards
- Write self-documenting code with meaningful names
- Keep methods focused and under 20 lines when possible
- Use guard clauses for early returns
- Implement comprehensive unit tests with 80%+ coverage
- Use structured logging with correlation IDs
- Handle exceptions gracefully with proper error responses
- Validate all inputs at API boundaries
- Use cancellation tokens for async operations

## Solution Structure
```
YourProject.sln
├── src/
│   ├── YourProject.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   ├── Interfaces/
│   │   └── Events/
│   ├── YourProject.Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── DTOs/
│   │   ├── Handlers/
│   │   ├── Validators/
│   │   ├── Mappings/
│   │   └── Interfaces/
│   ├── YourProject.Infrastructure/
│   │   ├── Data/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── Configurations/
│   └── YourProject.API/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Extensions/
│       └── Filters/
└── tests/
    ├── YourProject.UnitTests/
    ├── YourProject.IntegrationTests/
    └── YourProject.ArchitectureTests/
```

## Dependency Injection
- Register services per layer using extension methods (e.g., `AddApplicationServices()`, `AddInfrastructureServices()`)
- Use `AddScoped` for repositories and services
- Use `AddSingleton` for stateless utilities
- Use `AddTransient` for pipeline behaviors

## Configuration Management
- Use `IOptions<T>` for strongly typed configuration
- Environment-specific settings in `appsettings.{Environment}.json`
- Never hardcode connection strings, API keys, or secrets
- Use Azure Key Vault or environment variables for production secrets

## NuGet Package Standards
- MediatR for CQRS
- FluentValidation for validation
- AutoMapper for object mapping
- Serilog for structured logging
- Swashbuckle for API documentation
- xUnit + Moq + FluentAssertions for testing
- Entity Framework Core for data access

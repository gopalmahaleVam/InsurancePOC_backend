# Copilot Instructions - .NET Core Greenfield Development Toolkit

## Project Context

This is a comprehensive toolkit for building greenfield enterprise .NET Core applications from scratch. Follow the 21-step prompt workflow in `/prompts/` for systematic development, and use the architecture patterns defined here as the reference implementation standard.

## C# Language Standards

### Version & Features
- Use C# 14 features: file-scoped namespaces, raw strings, switch expressions, pattern matching
- Target .NET 8.0 or later
- Enable nullable reference types (`<Nullable>enable</Nullable>`)

### Naming Conventions
- **PascalCase**: Classes, methods, properties, public members
- **camelCase**: Private fields, local variables, parameters
- **Interfaces**: Prefix with "I" (e.g., `IUserService`)
- **Async methods**: Suffix with "Async" (e.g., `GetCustomerAsync`)

### Code Style
- File-scoped namespace declarations
- Use `nameof` instead of string literals for member names
- Pattern matching and switch expressions over traditional if/switch
- Use `is null` or `is not null` instead of `== null` or `!= null`
- Trust nullable annotations - don't add null checks when type system guarantees non-null
- Comments explain "why", not "what"
- No unused methods, parameters, or code

### Nullable Reference Types
- Declare variables non-nullable by default
- Check for null at entry points using `ArgumentNullException.ThrowIfNull(x)`
- For strings use `string.IsNullOrWhiteSpace(x)`

## Architecture Patterns

### Clean Architecture (Layered)
Follow this structure for all projects:

```
Domain (Entities, Value Objects, Interfaces, Events)
    ↓
Application (Commands, Queries, Handlers, DTOs, Validators)
    ↓
Infrastructure (Repositories, EF Core, External Services)
    ↓
API (Controllers, Middleware, Filters, Extensions)
```

### CQRS with MediatR
- Commands for write operations, Queries for read operations
- Pipeline behaviors for validation, logging, and exception handling
- Use FluentValidation integrated with MediatR pipeline

### Repository Pattern
- Create `IRepository<T>` interfaces in `Domain/Interfaces/`
- Implement in `Infrastructure/Repositories/`
- Use Unit of Work pattern for transaction management
- Services consume repositories, never direct DbContext

### Service Layer
- Create `IService` interfaces in `Application/Interfaces/`
- Implement in `Infrastructure/Services/` or `Application/Services/`
- Contains all business logic, validation, and orchestration

### DTOs (Data Transfer Objects)
- Separate request DTOs (e.g., `CreateCustomerRequest`) from response DTOs (e.g., `CustomerDto`)
- Use FluentValidation for complex rules, data annotations for simple ones
- Never expose domain entities directly through APIs
- Use AutoMapper profiles for entity ↔ DTO mapping

### Dependency Injection
- Register all services, repositories, and DbContext in extension methods per layer
- Use constructor injection exclusively
- Follow least-exposure rule: `private` > `internal` > `protected` > `public`

## Entity Framework Core

### Configuration
- Use SQL Server as primary database provider
- Connection strings in `appsettings.json`, never hardcoded
- Enable migrations: `dotnet ef migrations add [Name]`
- Apply migrations: `dotnet ef database update`

### Best Practices
- Use async methods: `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`
- Configure relationships with Fluent API in `OnModelCreating`
- Use `[Required]`, `[MaxLength]` attributes on entities
- Set delete behavior explicitly (typically `DeleteBehavior.Restrict`)
- Use decimal for monetary values with proper precision
- Use `.AsNoTracking()` for read-only queries

## Async Programming

### Requirements
- All I/O operations must be async
- Always await async calls - no fire-and-forget
- Accept `CancellationToken` parameters and pass through the call chain
- Use `ConfigureAwait(false)` in library code
- Name all async methods with "Async" suffix

### Avoid
- Sync-over-async (`Task.Result`, `.Wait()`)
- Async void (except event handlers)
- Unnecessary async/await wrappers

## API Design

### RESTful Conventions
- `GET /api/[entity]` - List all (with pagination)
- `GET /api/[entity]/{id}` - Get by ID
- `POST /api/[entity]` - Create
- `PUT /api/[entity]/{id}` - Update
- `DELETE /api/[entity]/{id}` - Delete
- `PATCH /api/[entity]/{id}/[action]` - Partial update

### Controllers
- Inherit from `ControllerBase` (not `Controller`)
- Use `[ApiController]` attribute
- Use `[Route("api/[controller]")]`
- Return `ActionResult<T>` or `IActionResult`
- Use proper HTTP status codes (200, 201, 204, 400, 404, 500)
- Use MediatR to dispatch commands/queries, keep controllers thin

### Response Patterns
```csharp
return Ok(data);              // 200
return CreatedAtAction(...);  // 201
return NoContent();           // 204
return BadRequest(error);     // 400
return NotFound();            // 404
```

## Authentication & Authorization

### JWT Bearer Tokens
- Configure in `Program.cs` with `AddAuthentication().AddJwtBearer()`
- Use `[Authorize]` attribute on controllers/actions
- Implement role-based or policy-based authorization
- Hash passwords with BCrypt or Identity's password hasher
- Never store passwords in plain text

### Security Rules
- Validate all inputs
- No secrets or credentials in code
- Use environment variables or Azure Key Vault for sensitive config
- Implement least privilege access
- Enable CORS appropriately

## Error Handling

### Exception Strategy
- Use specific exception types: `ArgumentException`, `InvalidOperationException`, `NotFoundException`
- Don't throw or catch base `Exception`
- No silent catches - log and rethrow or let bubble
- Implement global exception middleware for consistent error responses
- Use Problem Details (RFC 9457) for standardized error responses

### Validation
- Use FluentValidation on Commands/Requests
- Validate in service/handler layer before database operations
- Return meaningful error messages

## Testing

### Structure
- Separate test project: `[ProjectName].Tests`
- Mirror class structure: `CustomerService` → `CustomerServiceTests`
- Name tests by behavior: `WhenCustomerNotFoundThenThrowsException`

### Patterns
- Follow AAA pattern: Arrange, Act, Assert (no comments needed)
- One behavior per test
- Use xUnit as default framework
- Public instance test classes
- No branching/conditionals in tests

### Mocking
- Mock external dependencies only
- Never mock code in the solution under test
- Use Moq or NSubstitute

## Performance

### Optimization Rules
- Simple first, optimize hot paths when measured
- Stream large payloads, avoid loading everything into memory
- Async end-to-end, no blocking calls
- Implement pagination for large data sets
- Use caching strategically (in-memory, distributed, response caching)

### Database Performance
- Use `.AsNoTracking()` for read-only queries
- Avoid N+1 queries with `.Include()` or explicit loading
- Project to DTOs in queries to reduce data transfer
- Use indexes on frequently queried columns

## Logging & Monitoring

### Structured Logging
- Use `ILogger<T>` via dependency injection
- Log levels: Trace, Debug, Information, Warning, Error, Critical
- Include context: correlation IDs, user IDs, operation names
- Don't log sensitive data (passwords, tokens, PII)
- Use Serilog for structured logging

### Observability
- Implement health checks (`/health`, `/ready`)
- Add Application Insights or OpenTelemetry
- Monitor API performance, errors, and usage patterns

## Documentation

### API Documentation
- Enable Swagger/OpenAPI
- Document all endpoints with descriptions
- Include request/response examples
- Document authentication requirements
- Version APIs appropriately

## 21-Step Prompt Workflow

Follow the prompts in `/prompts/` in sequence for complete greenfield development:

1. **[01 - Solution Architecture Setup](../prompts/01-solution-architecture-setup.prompt.md)** - Clean Architecture, CQRS, DI setup
2. **[02 - Domain & Data Modeling](../prompts/02-domain-data-modeling.prompt.md)** - Entities, Value Objects, DTOs
3. **[03 - Repository & Data Access](../prompts/03-repository-data-access.prompt.md)** - Repository pattern, EF Core
4. **[04 - CQRS Application Layer](../prompts/04-cqrs-application-layer.prompt.md)** - Commands, Queries, Handlers
5. **[05 - API Layer Controllers](../prompts/05-api-layer-controllers.prompt.md)** - REST endpoints
6. **[06 - Service Communication](../prompts/06-service-communication.prompt.md)** - Inter-service communication
7. **[07 - Logging & Security](../prompts/07-logging-security-concerns.prompt.md)** - Auth, logging, middleware
8. **[08 - Testing & Quality](../prompts/08-testing-quality.prompt.md)** - Unit and integration tests
9. **[09 - Code Quality Standards](../prompts/09-code-quality-standards.prompt.md)** - Standards enforcement
10. **[10 - Unit Test Creation](../prompts/10-unit-test-creation.prompt.md)** - Detailed unit tests
11. **[11 - Integration Testing](../prompts/11-integration-testing.prompt.md)** - Integration test setup
12. **[12 - Performance Optimization](../prompts/12-performance-optimization.prompt.md)** - Caching, async, DB tuning
13. **[13 - Microservices Architecture](../prompts/13-microservices-architecture.prompt.md)** - Service decomposition
14. **[14 - Event-Driven Architecture](../prompts/14-event-driven-architecture.prompt.md)** - Events, messaging
15. **[15 - Background Services](../prompts/15-background-services.prompt.md)** - Hosted services, workers
16. **[16 - API Documentation](../prompts/16-api-documentation.prompt.md)** - Swagger, versioning
17. **[17 - Monitoring & Observability](../prompts/17-monitoring-observability.prompt.md)** - Telemetry, health checks
18. **[18 - Deployment Strategies](../prompts/18-deployment-strategies.prompt.md)** - Docker, CI/CD, Azure
19. **[19 - Data Migration Tools](../prompts/19-data-migration-tools.prompt.md)** - EF migrations, data seeding
20. **[20 - Code Generation Tools](../prompts/20-code-generation-tools.prompt.md)** - Scaffolding, templates
21. **[21 - Code Refactoring Automation](../prompts/21-code-refactoring-automation.prompt.md)** - Refactoring patterns

## Project Setup

### Common NuGet Packages
- **CQRS**: MediatR, MediatR.Extensions.Microsoft.DependencyInjection
- **Validation**: FluentValidation, FluentValidation.AspNetCore
- **Mapping**: AutoMapper, AutoMapper.Extensions.Microsoft.DependencyInjection
- **Database**: EF Core SqlServer + EF Core Tools
- **Authentication**: JwtBearer, Identity
- **API Documentation**: Swashbuckle (Swagger)
- **Logging**: Serilog, Serilog.AspNetCore
- **Testing**: xUnit, Moq, FluentAssertions

### Program.cs Configuration
- Register DbContext with connection string
- Add authentication/authorization
- Register repositories, services, and MediatR
- Enable Swagger in development
- Configure CORS if needed
- Add exception handling middleware
- Add health checks

## Final Checklist

Before completing any feature:
- ✅ Code compiles without warnings
- ✅ All async methods use proper cancellation
- ✅ Nullable reference types handled correctly
- ✅ Error handling implemented
- ✅ Tests written and passing
- ✅ XML documentation on public APIs
- ✅ No hardcoded secrets or connection strings
- ✅ Follows Clean Architecture pattern
- ✅ Logging added for important operations
- ✅ Security considerations addressed
- ✅ SOLID principles applied

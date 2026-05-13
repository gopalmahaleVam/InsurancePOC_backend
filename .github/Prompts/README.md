# .NET Core Greenfield Development - Step-by-Step Guide

## Overview
This guide breaks down the development of any enterprise .NET Core application into 21 focused, sequential steps. Each step produces one specific component following Clean Architecture, CQRS, and DDD principles.

## Step-by-Step Prompts

### Foundation
1. **[Step 1: Solution Architecture Setup](../../prompts/01-solution-architecture-setup.prompt.md)**
   - Create Clean Architecture solution structure
   - Set up CQRS with MediatR
   - Configure dependency injection per layer
   - Add health checks and Serilog logging

2. **[Step 2: Domain & Data Modeling](../../prompts/02-domain-data-modeling.prompt.md)**
   - Create rich domain entities with business logic
   - Define Value Objects and Domain Enums
   - Create Request/Response DTOs
   - Set up AutoMapper profiles and FluentValidation validators

3. **[Step 3: Repository & Data Access](../../prompts/03-repository-data-access.prompt.md)**
   - Implement Repository Pattern with interfaces
   - Configure Entity Framework Core DbContext
   - Set up Unit of Work pattern
   - Create database migrations

4. **[Step 4: CQRS Application Layer](../../prompts/04-cqrs-application-layer.prompt.md)**
   - Implement Commands and Queries
   - Create MediatR Handlers
   - Add pipeline behaviors (validation, logging, transactions)
   - Wire FluentValidation into MediatR pipeline

### API Layer
5. **[Step 5: API Layer Controllers](../../prompts/05-api-layer-controllers.prompt.md)**
   - Create REST API controllers
   - Dispatch commands/queries via MediatR
   - Implement proper HTTP status codes
   - Add request/response handling

6. **[Step 6: Service Communication](../../prompts/06-service-communication.prompt.md)**
   - Set up inter-service communication (HTTP clients, gRPC)
   - Implement retry policies with Polly
   - Configure typed HTTP clients
   - Handle service-to-service authentication

### Security & Quality
7. **[Step 7: Logging & Security](../../prompts/07-logging-security-concerns.prompt.md)**
   - Implement JWT authentication and authorization
   - Configure Serilog with enrichers and sinks
   - Add global exception handling middleware
   - Set up correlation ID tracking

8. **[Step 8: Testing & Quality](../../prompts/08-testing-quality.prompt.md)**
   - Set up test projects (Unit, Integration, Architecture)
   - Write unit tests for handlers and services
   - Configure integration test infrastructure
   - Add architecture tests with NetArchTest

9. **[Step 9: Code Quality Standards](../../prompts/09-code-quality-standards.prompt.md)**
   - Configure EditorConfig and StyleCop
   - Set up code analysis rules
   - Add nullable reference types enforcement
   - Configure build warnings as errors

### Testing
10. **[Step 10: Unit Test Creation](../../prompts/10-unit-test-creation.prompt.md)**
    - Write comprehensive unit tests for domain logic
    - Test command/query handlers
    - Mock repositories and external services
    - Achieve 80%+ code coverage

11. **[Step 11: Integration Testing](../../prompts/11-integration-testing.prompt.md)**
    - Set up WebApplicationFactory for API tests
    - Configure test database (SQLite or TestContainers)
    - Write end-to-end API integration tests
    - Test authentication flows

12. **[Step 12: Performance Optimization](../../prompts/12-performance-optimization.prompt.md)**
    - Implement caching (in-memory, distributed, response)
    - Optimize EF Core queries (AsNoTracking, projections)
    - Add pagination for large datasets
    - Profile and benchmark hot paths

### Advanced Features
13. **[Step 13: Microservices Architecture](../../prompts/13-microservices-architecture.prompt.md)**
    - Decompose monolith into microservices
    - Set up API Gateway
    - Implement service discovery
    - Configure distributed tracing

14. **[Step 14: Event-Driven Architecture](../../prompts/14-event-driven-architecture.prompt.md)**
    - Implement domain events
    - Set up message broker (RabbitMQ, Azure Service Bus)
    - Create event handlers and consumers
    - Implement outbox pattern for reliability

15. **[Step 15: Background Services](../../prompts/15-background-services.prompt.md)**
    - Create IHostedService implementations
    - Set up recurring background jobs
    - Implement worker services
    - Add job scheduling with Quartz.NET or Hangfire

### Documentation & Observability
16. **[Step 16: API Documentation](../../prompts/16-api-documentation.prompt.md)**
    - Configure Swagger/OpenAPI with Swashbuckle
    - Document all endpoints with XML comments
    - Add request/response examples
    - Set up API versioning

17. **[Step 17: Monitoring & Observability](../../prompts/17-monitoring-observability.prompt.md)**
    - Add Application Insights or OpenTelemetry
    - Implement health checks (`/health`, `/ready`)
    - Set up metrics and distributed tracing
    - Configure alerting and dashboards

### Deployment & Maintenance
18. **[Step 18: Deployment Strategies](../../prompts/18-deployment-strategies.prompt.md)**
    - Containerize with Docker (multi-stage builds)
    - Set up CI/CD pipelines (GitHub Actions / Azure DevOps)
    - Deploy to Azure App Service or Azure Container Apps
    - Configure environment-specific settings

19. **[Step 19: Data Migration Tools](../../prompts/19-data-migration-tools.prompt.md)**
    - Manage EF Core migrations
    - Implement data seeding strategies
    - Handle schema versioning
    - Create rollback strategies

20. **[Step 20: Code Generation Tools](../../prompts/20-code-generation-tools.prompt.md)**
    - Set up T4 templates or source generators
    - Create scaffolding scripts for new features
    - Automate boilerplate generation
    - Build custom dotnet CLI tools

21. **[Step 21: Code Refactoring Automation](../../prompts/21-code-refactoring-automation.prompt.md)**
    - Identify and resolve code smells
    - Apply automated refactoring patterns
    - Enforce architecture rules
    - Modernize legacy patterns to current standards

## Usage Instructions

1. **Sequential Development**: Follow steps 1-9 in order for a complete base application
2. **Focused Implementation**: Each step produces one specific component
3. **Incremental Testing**: Test after each step before proceeding
4. **Flexible Approach**: Steps 10-21 can be applied as needed based on requirements
5. **Customization**: Replace all `[placeholders]` with your specific values

## Before You Start

### Prepare Your Placeholders
- `[YourNamespace]` - Your project namespace (e.g., `Acme.Insurance`)
- `[YourProjectName]` - Your solution/project name
- `[EntityName]` - Your domain entity names (e.g., `Policy`, `Customer`)
- `[DatabaseProvider]` - Your database provider (SqlServer, PostgreSQL, SQLite)
- `[YourConnectionString]` - Your database connection string

## Project Structure

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
│   │   └── Mappings/
│   ├── YourProject.Infrastructure/
│   │   ├── Data/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── Configurations/
│   └── YourProject.API/
│       ├── Controllers/
│       ├── Middleware/
│       └── Extensions/
└── tests/
    ├── YourProject.UnitTests/
    ├── YourProject.IntegrationTests/
    └── YourProject.ArchitectureTests/
```

## Architecture Pattern

```
Client Request
    ↓
Controller (HTTP handling — thin, dispatches via MediatR)
    ↓
Handler (CQRS — Command/Query Handler)
    ↓
Service (Business logic — optional for complex orchestration)
    ↓
Repository (Data access abstraction)
    ↓
DbContext (EF Core)
    ↓
Database
```

## Final Result
A complete .NET 8 enterprise application that:
- Follows Clean Architecture with clear layer separation
- Uses CQRS with MediatR for scalable command/query handling
- Implements Repository Pattern for clean data access
- Includes comprehensive API documentation (Swagger)
- Has full test coverage (Unit, Integration, Architecture)
- Is containerized and CI/CD ready
- Follows enterprise-grade .NET patterns and best practices

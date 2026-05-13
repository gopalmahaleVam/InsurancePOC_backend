---
description: 'Scaffold a Clean Architecture .NET Core solution with CQRS, MediatR, DI, and health checks.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Scaffold Clean Architecture Solution

## Mission
Generate a complete .NET Core solution structure following Clean Architecture with CQRS, MediatR pipeline, FluentValidation, Serilog, and health checks for the project `${input:projectName:YourProject}` under namespace `${input:namespace:YourCompany.YourProject}`.

## Scope & Preconditions
- .NET 8 SDK must be installed
- Target database: `${input:databaseProvider:SqlServer}` (SqlServer / PostgreSQL / SQLite)
- Solution will be created at: `${input:outputPath:./src}`
- If `projectName` or `namespace` is not provided, stop and ask the user before proceeding

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Name of the solution and root project | `YourProject` |
| `namespace` | Root namespace for all projects | `YourCompany.YourProject` |
| `databaseProvider` | EF Core database provider | `SqlServer` |
| `outputPath` | Where to create the solution | `./src` |

## Workflow

### Step 1 — Create Solution & Projects
```bash
dotnet new sln -n ${input:projectName}
dotnet new classlib -n ${input:namespace}.Domain -f net8.0
dotnet new classlib -n ${input:namespace}.Application -f net8.0
dotnet new classlib -n ${input:namespace}.Infrastructure -f net8.0
dotnet new webapi -n ${input:namespace}.API -f net8.0
dotnet new xunit -n ${input:namespace}.UnitTests -f net8.0
dotnet new xunit -n ${input:namespace}.ArchitectureTests -f net8.0
```

### Step 2 — Add Projects to Solution
```bash
dotnet sln add **/*.csproj
```

### Step 3 — Add Project References
```bash
dotnet add ${input:namespace}.Application reference ${input:namespace}.Domain
dotnet add ${input:namespace}.Infrastructure reference ${input:namespace}.Application
dotnet add ${input:namespace}.Infrastructure reference ${input:namespace}.Domain
dotnet add ${input:namespace}.API reference ${input:namespace}.Application
dotnet add ${input:namespace}.API reference ${input:namespace}.Infrastructure
dotnet add ${input:namespace}.UnitTests reference ${input:namespace}.Application
dotnet add ${input:namespace}.UnitTests reference ${input:namespace}.Domain
dotnet add ${input:namespace}.ArchitectureTests reference ${input:namespace}.API
dotnet add ${input:namespace}.ArchitectureTests reference ${input:namespace}.Application
dotnet add ${input:namespace}.ArchitectureTests reference ${input:namespace}.Domain
dotnet add ${input:namespace}.ArchitectureTests reference ${input:namespace}.Infrastructure
```

### Step 4 — Create Folder Structure
Create the following folders:

**Domain**
- `Entities/`, `ValueObjects/`, `Enums/`, `Interfaces/`, `Events/`, `Exceptions/`

**Application**
- `Common/Behaviours/`, `Common/Models/`, `Common/Interfaces/`, `Common/Mappings/`

**Infrastructure**
- `Data/`, `Data/Configurations/`, `Repositories/`, `Services/`

**API**
- `Controllers/`, `Middleware/`, `Extensions/`

### Step 5 — Generate Core Files

Generate `Program.cs` in `${input:namespace}.API`:
```csharp
using ${input:namespace}.API.Middleware;
using ${input:namespace}.Application;
using ${input:namespace}.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

Generate `DependencyInjection.cs` in `${input:namespace}.Application`:
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ${input:namespace}.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        return services;
    }
}
```

Generate `DependencyInjection.cs` in `${input:namespace}.Infrastructure`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ${input:namespace}.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}
```

### Step 6 — Install NuGet Packages
```bash
# Application
dotnet add ${input:namespace}.Application package MediatR
dotnet add ${input:namespace}.Application package FluentValidation.DependencyInjectionExtensions
dotnet add ${input:namespace}.Application package AutoMapper.Extensions.Microsoft.DependencyInjection

# Infrastructure
dotnet add ${input:namespace}.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add ${input:namespace}.Infrastructure package Microsoft.EntityFrameworkCore.Tools

# API
dotnet add ${input:namespace}.API package Swashbuckle.AspNetCore
dotnet add ${input:namespace}.API package Microsoft.AspNetCore.Diagnostics.HealthChecks

# Tests
dotnet add ${input:namespace}.UnitTests package Moq
dotnet add ${input:namespace}.UnitTests package FluentAssertions
dotnet add ${input:namespace}.ArchitectureTests package NetArchTest.Rules
```

### Step 7 — Verify Build
```bash
dotnet build
```

## Output Expectations
- Solution file: `${input:outputPath}/${input:projectName}.sln`
- 4 source projects + 2 test projects all compile without errors
- `dotnet build` exits with code 0
- Folder structure matches the layout defined in Step 4

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] All 6 projects present in solution
- [ ] Project references follow inward dependency flow (Domain has zero references)
- [ ] `AddApplicationServices()` and `AddInfrastructureServices()` registered in `Program.cs`
- [ ] MediatR + FluentValidation pipeline behaviours registered
- [ ] `/health` endpoint mapped
- [ ] Swagger enabled for Development environment
- [ ] No hardcoded connection strings

### Failure Triggers
- Stop if `dotnet build` fails — fix errors before proceeding to Step 2 (Domain Modeling)
- Stop if project references create circular dependencies
- Request `outputPath` from user if not provided

### Next Step
Proceed to [02-domain-data-modeling.prompt.md](./02-domain-data-modeling.prompt.md) once build succeeds.

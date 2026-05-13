---
description: 'Create and apply EF Core migrations, add a data seeder, and document the rollback strategy.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Manage Data Migrations

## Mission
Create the initial EF Core migration, apply it to the database, add a `DataSeeder` for development seed data, and document the rollback strategy for `${input:projectName:YourProject}`.

## Scope & Preconditions
- Steps 01–18 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Connection string must be set in `appsettings.json` before running migrations
- If connection string is missing, stop and ask the user to configure it first

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `migrationName` | Name for the initial migration | `InitialCreate` |

## Workflow

### Step 1 — Install EF Core Tools
```bash
dotnet tool install --global dotnet-ef
```

### Step 2 — Create Initial Migration
```bash
dotnet ef migrations add ${input:migrationName} \
  --project src/${input:namespace}.Infrastructure \
  --startup-project src/${input:namespace}.API
```

Verify migration files created under `src/${input:namespace}.Infrastructure/Migrations/`.

### Step 3 — Apply Migration to Database
```bash
dotnet ef database update \
  --project src/${input:namespace}.Infrastructure \
  --startup-project src/${input:namespace}.API
```

### Step 4 — Create DataSeeder
Create `src/${input:namespace}.Infrastructure/Data/DataSeeder.cs`:
```csharp
using ${input:namespace}.Domain.Entities;
using ${input:namespace}.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ${input:namespace}.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Database.CanConnectAsync())
        {
            await context.Database.MigrateAsync();
        }

        if (!await context.Set<Customer>().AnyAsync())
        {
            var customers = new[]
            {
                new Customer("Seed Customer 1", new Email("seed1@example.com"), "0000000001", DateTime.UtcNow.AddYears(-30)),
                new Customer("Seed Customer 2", new Email("seed2@example.com"), "0000000002", DateTime.UtcNow.AddYears(-25))
            };

            context.Set<Customer>().AddRange(customers);
            await context.SaveChangesAsync();
            logger.LogInformation("Seed data applied");
        }
    }
}
```

### Step 5 — Call DataSeeder in Program.cs (Development Only)
```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DataSeeder.SeedAsync(context, logger);
}
```

### Step 6 — Verify Migration Applied
```bash
dotnet ef migrations list \
  --project src/${input:namespace}.Infrastructure \
  --startup-project src/${input:namespace}.API
```

Confirm `${input:migrationName}` shows as `Applied`.

### Rollback Strategy
To roll back the last migration:
```bash
# Revert database to previous migration
dotnet ef database update PreviousMigrationName \
  --project src/${input:namespace}.Infrastructure \
  --startup-project src/${input:namespace}.API

# Remove the migration file
dotnet ef migrations remove \
  --project src/${input:namespace}.Infrastructure \
  --startup-project src/${input:namespace}.API
```

## Output Expectations
- Migration files in `Infrastructure/Migrations/`
- Database schema created with all entity tables
- `DataSeeder.cs` in `Infrastructure/Data/`
- Seed data applied in Development environment on startup
- `dotnet ef migrations list` shows `${input:migrationName}` as Applied

## Quality Assurance

### Validation Checklist
- [ ] Migration files generated in `Infrastructure/Migrations/`
- [ ] `dotnet ef database update` succeeds
- [ ] `DataSeeder` only runs in Development environment
- [ ] `DataSeeder` is idempotent — safe to run multiple times
- [ ] Connection string not hardcoded in migration commands
- [ ] Rollback commands documented and tested

### Failure Triggers
- Stop if `dotnet ef migrations add` fails — check `DbContext` registration and connection string
- Stop if `dotnet ef database update` fails — verify SQL Server is running and connection string is correct
- Request connection string from user if not configured in `appsettings.json`

### Next Step
Proceed to [20-code-generation-tools.prompt.md](./20-code-generation-tools.prompt.md) once migration is applied and seed data is visible.

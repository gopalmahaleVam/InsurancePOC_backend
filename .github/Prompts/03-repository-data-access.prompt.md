---
description: 'Generate generic and specific repositories, Unit of Work, DbContext, and EF Core entity configurations.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Generate Repository & Data Access Layer

## Mission
Create the Infrastructure data access layer for `${input:projectName:YourProject}` — generic `IRepository<T>`, entity-specific repositories, `IUnitOfWork`, `ApplicationDbContext`, and EF Core Fluent API configurations.

## Scope & Preconditions
- Steps 01 and 02 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity to generate repository for: `${input:entityName:Customer}`
- Database provider: `${input:databaseProvider:SqlServer}`
- Connection string key: `${input:connectionStringKey:DefaultConnection}`
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Entity to generate repository for | `Customer` |
| `databaseProvider` | EF Core provider | `SqlServer` |
| `connectionStringKey` | Key in appsettings.json | `DefaultConnection` |

## Workflow

### Step 1 — Create Generic Repository Interface
Create `src/${input:namespace}.Domain/Interfaces/IRepository.cs`:
```csharp
using ${input:namespace}.Domain.Entities;

namespace ${input:namespace}.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
```

### Step 2 — Create Entity-Specific Repository Interface
Create `src/${input:namespace}.Domain/Interfaces/I${input:entityName}Repository.cs`:
```csharp
using ${input:namespace}.Domain.Entities;

namespace ${input:namespace}.Domain.Interfaces;

public interface I${input:entityName}Repository : IRepository<${input:entityName}>
{
    Task<${input:entityName}?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
```

### Step 3 — Create Unit of Work Interface
Create `src/${input:namespace}.Domain/Interfaces/IUnitOfWork.cs`:
```csharp
namespace ${input:namespace}.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    I${input:entityName}Repository ${input:entityName}s { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### Step 4 — Create DbContext
Create `src/${input:namespace}.Infrastructure/Data/ApplicationDbContext.cs`:
```csharp
using ${input:namespace}.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ${input:namespace}.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<${input:entityName}> ${input:entityName}s => Set<${input:entityName}>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### Step 5 — Create EF Core Entity Configuration
Create `src/${input:namespace}.Infrastructure/Data/Configurations/${input:entityName}Configuration.cs`:
```csharp
using ${input:namespace}.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ${input:namespace}.Infrastructure.Data.Configurations;

public class ${input:entityName}Configuration : IEntityTypeConfiguration<${input:entityName}>
{
    public void Configure(EntityTypeBuilder<${input:entityName}> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasQueryFilter(e => !e.IsDeleted);
        // Add entity-specific configuration here
    }
}
```

### Step 6 — Create Generic Repository Implementation
Create `src/${input:namespace}.Infrastructure/Repositories/Repository.cs`:
```csharp
using ${input:namespace}.Domain.Entities;
using ${input:namespace}.Domain.Interfaces;
using ${input:namespace}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ${input:namespace}.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => entity.MarkAsDeleted();

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
}
```

### Step 7 — Create Entity-Specific Repository Implementation
Create `src/${input:namespace}.Infrastructure/Repositories/${input:entityName}Repository.cs`.

### Step 8 — Create Unit of Work Implementation
Create `src/${input:namespace}.Infrastructure/Repositories/UnitOfWork.cs`.

### Step 9 — Register in DependencyInjection.cs
Update `src/${input:namespace}.Infrastructure/DependencyInjection.cs` to register `DbContext`, repositories, and `IUnitOfWork`.

### Step 10 — Add Migration
```bash
dotnet ef migrations add InitialCreate --project src/${input:namespace}.Infrastructure --startup-project src/${input:namespace}.API
```

### Step 11 — Verify Build
```bash
dotnet build
```

## Output Expectations
- `IRepository<T>` and `I${input:entityName}Repository` in `Domain/Interfaces/`
- `IUnitOfWork` in `Domain/Interfaces/`
- `ApplicationDbContext.cs` in `Infrastructure/Data/`
- `${input:entityName}Configuration.cs` in `Infrastructure/Data/Configurations/`
- `Repository<T>.cs` and `${input:entityName}Repository.cs` in `Infrastructure/Repositories/`
- `UnitOfWork.cs` in `Infrastructure/Repositories/`
- Migration file created under `Infrastructure/Migrations/`
- `dotnet build` exits with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] `IRepository<T>` and `IUnitOfWork` defined in Domain (not Infrastructure)
- [ ] Global query filter `!e.IsDeleted` applied for soft delete
- [ ] All repository methods are async with `CancellationToken`
- [ ] Connection string read from configuration — not hardcoded
- [ ] Migration file generated successfully
- [ ] `UnitOfWork` registered as `IUnitOfWork` with `AddScoped`

### Failure Triggers
- Stop if `dotnet ef migrations add` fails — check DbContext registration
- Stop if Infrastructure references API project — violates Clean Architecture
- Request `entityName` from user if not provided

### Next Step
Proceed to [04-cqrs-application-layer.prompt.md](./04-cqrs-application-layer.prompt.md) once build and migration succeed.

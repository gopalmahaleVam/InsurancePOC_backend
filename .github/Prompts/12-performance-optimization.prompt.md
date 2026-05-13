---
description: 'Add IMemoryCache to query handlers, AsNoTracking to read queries, and response compression to the API.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Implement Performance Optimizations

## Mission
Optimize `${input:projectName:YourProject}` by adding `IMemoryCache` to read query handlers, applying `.AsNoTracking()` to all read-only EF Core queries, enabling response compression, and adding pagination support.

## Scope & Preconditions
- Steps 01–11 must be completed and `dotnet test` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity to optimize: `${input:entityName:Customer}`
- Cache expiration: `${input:cacheExpirationMinutes:5}` minutes
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Entity to optimize | `Customer` |
| `cacheExpirationMinutes` | Cache TTL in minutes | `5` |

## Workflow

### Step 1 — Add IMemoryCache to Query Handler
Update `src/${input:namespace}.Application/${input:entityName}s/Queries/Get${input:entityName}Queries.cs` — inject `IMemoryCache` into `Get${input:entityName}ByIdHandler`:
```csharp
using Microsoft.Extensions.Caching.Memory;

public class Get${input:entityName}ByIdHandler : IRequestHandler<Get${input:entityName}ByIdQuery, Result<${input:entityName}Dto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public Get${input:entityName}ByIdHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<${input:entityName}Dto>> Handle(Get${input:entityName}ByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"${input:entityName}_{request.Id}";

        if (_cache.TryGetValue(cacheKey, out ${input:entityName}Dto? cached))
            return Result<${input:entityName}Dto>.Success(cached!);

        var entity = await _unitOfWork.${input:entityName}s.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result<${input:entityName}Dto>.Failure($"${input:entityName} with ID {request.Id} was not found.");

        var dto = new ${input:entityName}Dto(/* map properties */);
        _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(${input:cacheExpirationMinutes}));

        return Result<${input:entityName}Dto>.Success(dto);
    }
}
```

### Step 2 — Apply AsNoTracking to GetAll Query
Update `GetAll${input:entityName}sHandler` in the repository to use `.AsNoTracking()`:
```csharp
public async Task<IEnumerable<${input:entityName}>> GetAllAsync(CancellationToken cancellationToken = default) =>
    await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
```

### Step 3 — Add Pagination Support
Add `PagedResult<T>` model to `src/${input:namespace}.Application/Common/Models/PagedResult.cs`:
```csharp
namespace ${input:namespace}.Application.Common.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
```

Add paged query to repository:
```csharp
public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
{
    var totalCount = await _dbSet.CountAsync(cancellationToken);
    var items = await _dbSet
        .AsNoTracking()
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return new PagedResult<T> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
}
```

### Step 4 — Enable Response Compression
Update `src/${input:namespace}.API/Program.cs`:
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// After app.Build():
app.UseResponseCompression();
```

### Step 5 — Register IMemoryCache
Ensure `Program.cs` includes:
```csharp
builder.Services.AddMemoryCache();
```

### Step 6 — Verify Build and Tests
```bash
dotnet build
dotnet test
```

## Output Expectations
- `Get${input:entityName}ByIdHandler` uses `IMemoryCache` with `${input:cacheExpirationMinutes}` minute TTL
- `GetAllAsync` uses `.AsNoTracking()`
- `PagedResult<T>` model created
- `GetPagedAsync` method added to repository
- Response compression enabled
- `dotnet build` and `dotnet test` exit with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] `dotnet test` passes with no failures
- [ ] `IMemoryCache` injected — not `IDistributedCache` (keep it simple)
- [ ] `.AsNoTracking()` applied to all read-only queries
- [ ] `PagedResult<T>` has `TotalPages` computed property
- [ ] Response compression enabled for HTTPS
- [ ] Cache key is deterministic and entity-specific

### Failure Triggers
- Stop if `dotnet test` fails after adding caching — check mock setup for `IMemoryCache`
- Stop if `dotnet build` fails — check `AddMemoryCache()` is registered
- Request `entityName` from user if not provided

### Next Step
Proceed to [13-microservices-architecture.prompt.md](./13-microservices-architecture.prompt.md) once build and tests pass.

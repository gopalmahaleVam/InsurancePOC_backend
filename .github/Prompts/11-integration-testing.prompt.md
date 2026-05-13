---
description: 'Generate integration tests using WebApplicationFactory with in-memory database for API endpoint and repository testing.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Generate Integration Tests

## Mission
Create API integration tests using `WebApplicationFactory<Program>` with an in-memory database, and repository integration tests for `${input:entityName:Customer}` in `${input:projectName:YourProject}`.

## Scope & Preconditions
- Steps 01–10 must be completed and `dotnet test` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity under test: `${input:entityName:Customer}`
- Integration tests use `Microsoft.EntityFrameworkCore.InMemory`
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Entity to generate integration tests for | `Customer` |

## Workflow

### Step 1 — Install Integration Test Packages
```bash
dotnet add tests/${input:namespace}.UnitTests package Microsoft.AspNetCore.Mvc.Testing
dotnet add tests/${input:namespace}.UnitTests package Microsoft.EntityFrameworkCore.InMemory
```

### Step 2 — Create Integration Test Base
Create `tests/${input:namespace}.UnitTests/Integration/IntegrationTestBase.cs`:
```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ${input:namespace}.Infrastructure.Data;

namespace ${input:namespace}.UnitTests.Integration;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly ApplicationDbContext Context;
    private readonly IServiceScope _scope;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        var app = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
            });
        });

        Client = app.CreateClient();
        _scope = app.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        _scope.Dispose();
        Client.Dispose();
    }
}
```

### Step 3 — Create API Integration Tests
Create `tests/${input:namespace}.UnitTests/Integration/${input:entityName}sApiTests.cs`:
```csharp
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using ${input:namespace}.Application.${input:entityName}s.Commands;
using Xunit;

namespace ${input:namespace}.UnitTests.Integration;

public class ${input:entityName}sApiTests : IntegrationTestBase
{
    public ${input:entityName}sApiTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/${input:entityName}s".ToLowerInvariant());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        var command = new Create${input:entityName}Command(/* valid args */);

        var response = await Client.PostAsJsonAsync("/api/${input:entityName}s".ToLowerInvariant(), command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/${input:entityName}s/99999".ToLowerInvariant());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        var command = new Create${input:entityName}Command(/* invalid args */);

        var response = await Client.PostAsJsonAsync("/api/${input:entityName}s".ToLowerInvariant(), command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
```

### Step 4 — Create Repository Integration Tests
Create `tests/${input:namespace}.UnitTests/Integration/${input:entityName}RepositoryTests.cs`:
```csharp
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ${input:namespace}.Domain.Entities;
using ${input:namespace}.Domain.ValueObjects;
using ${input:namespace}.Infrastructure.Data;
using ${input:namespace}.Infrastructure.Repositories;
using Xunit;

namespace ${input:namespace}.UnitTests.Integration;

public class ${input:entityName}RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ${input:entityName}RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_PersistsEntity()
    {
        var repo = new ${input:entityName}Repository(_context);
        var entity = new ${input:entityName}(/* valid args */);

        await repo.AddAsync(entity);
        await _context.SaveChangesAsync();

        var saved = await _context.${input:entityName}s.FirstOrDefaultAsync(e => e.Id == entity.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
    {
        var repo = new ${input:entityName}Repository(_context);
        var entity = new ${input:entityName}(/* valid args */);
        await repo.AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Remove_SoftDeletesEntity()
    {
        var repo = new ${input:entityName}Repository(_context);
        var entity = new ${input:entityName}(/* valid args */);
        await repo.AddAsync(entity);
        await _context.SaveChangesAsync();

        repo.Remove(entity);
        await _context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(entity.Id);
        result.Should().BeNull(); // Global query filter excludes soft-deleted
    }

    public void Dispose() => _context.Dispose();
}
```

### Step 5 — Run Integration Tests
```bash
dotnet test --filter "Category=Integration"
```

### Step 6 — Run All Tests
```bash
dotnet test
```

## Output Expectations
- `IntegrationTestBase.cs` in `UnitTests/Integration/`
- `${input:entityName}sApiTests.cs` in `UnitTests/Integration/`
- `${input:entityName}RepositoryTests.cs` in `UnitTests/Integration/`
- All tests pass: `dotnet test` exits with code 0
- GET, POST, and error scenarios covered for the API

## Quality Assurance

### Validation Checklist
- [ ] `dotnet test` passes with no failures
- [ ] Each integration test uses a unique in-memory database (`Guid.NewGuid()`)
- [ ] API tests cover 200, 201, 400, and 404 status codes
- [ ] Repository tests verify soft delete via global query filter
- [ ] No real database or external services used in tests
- [ ] `IDisposable` implemented to clean up scope and client

### Failure Triggers
- Stop if `dotnet test` fails — check `Program.cs` is accessible from test project
- Stop if in-memory database tests share state — ensure unique DB name per test
- Request `entityName` from user if not provided

### Next Step
Proceed to [12-performance-optimization.prompt.md](./12-performance-optimization.prompt.md) once all integration tests pass.

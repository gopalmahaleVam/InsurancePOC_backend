---
description: 'Extract a bounded context into a standalone microservice with its own DbContext, API, and service discovery registration.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Extract Microservice

## Mission
Extract the `${input:serviceName:Customer}` bounded context from `${input:projectName:YourProject}` into a standalone microservice with its own solution, DbContext, typed HTTP client for inter-service calls, and health check endpoint.

## Scope & Preconditions
- Steps 01–12 must be completed and `dotnet test` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Service to extract: `${input:serviceName:Customer}`
- Port for new service: `${input:servicePort:5002}`
- If `serviceName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Parent solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `serviceName` | Bounded context to extract | `Customer` |
| `servicePort` | HTTPS port for new service | `5002` |

## Workflow

### Step 1 — Create New Microservice Solution
```bash
mkdir ${input:serviceName}Service
cd ${input:serviceName}Service
dotnet new sln -n ${input:serviceName}Service
dotnet new webapi -n ${input:namespace}.${input:serviceName}Service.API -f net8.0
dotnet sln add **/*.csproj
```

### Step 2 — Copy Domain & Application Layers
Copy relevant domain entities, interfaces, commands, queries, and handlers from the parent solution into the new service. Keep only what belongs to the `${input:serviceName}` bounded context.

### Step 3 — Create Service Contract Interface
Create `src/${input:namespace}.Application/Common/Interfaces/I${input:serviceName}ServiceClient.cs` in the **calling** service:
```csharp
namespace ${input:namespace}.Application.Common.Interfaces;

public interface I${input:serviceName}ServiceClient
{
    Task<${input:serviceName}Dto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
```

### Step 4 — Create Typed HTTP Client
Create `src/${input:namespace}.Infrastructure/Services/${input:serviceName}ServiceClient.cs`:
```csharp
using ${input:namespace}.Application.Common.Interfaces;
using System.Net.Http.Json;

namespace ${input:namespace}.Infrastructure.Services;

public class ${input:serviceName}ServiceClient : I${input:serviceName}ServiceClient
{
    private readonly HttpClient _httpClient;

    public ${input:serviceName}ServiceClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<${input:serviceName}Dto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/${input:serviceName}s/{id}".ToLowerInvariant(), cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<${input:serviceName}Dto>(cancellationToken: cancellationToken);
    }
}
```

### Step 5 — Register HTTP Client in DependencyInjection.cs
```csharp
services.AddHttpClient<I${input:serviceName}ServiceClient, ${input:serviceName}ServiceClient>(client =>
{
    client.BaseAddress = new Uri(configuration["${input:serviceName}Service:BaseUrl"]
        ?? throw new InvalidOperationException("${input:serviceName}Service:BaseUrl not configured"));
})
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
```

### Step 6 — Add Health Check to New Service
In the new service's `Program.cs`:
```csharp
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
```

### Step 7 — Add Service URL to appsettings.json
```json
"${input:serviceName}Service": {
  "BaseUrl": "https://localhost:${input:servicePort}"
}
```

### Step 8 — Verify Both Services Build
```bash
dotnet build
```

## Output Expectations
- New `${input:serviceName}Service` solution created
- `I${input:serviceName}ServiceClient` interface in calling service's `Application/Common/Interfaces/`
- `${input:serviceName}ServiceClient` typed HTTP client in `Infrastructure/Services/`
- HTTP client registered with retry policy
- `/health` endpoint in new service
- `dotnet build` exits with code 0 for both solutions

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds for both solutions
- [ ] Service client interface defined in Application layer
- [ ] HTTP client uses retry policy with exponential backoff
- [ ] Base URL read from configuration — not hardcoded
- [ ] New service has `/health` endpoint
- [ ] No circular dependencies between services

### Failure Triggers
- Stop if `BaseUrl` configuration is missing — request it from the user
- Stop if `dotnet build` fails for either solution
- Request `serviceName` from user if not provided

### Next Step
Proceed to [14-event-driven-architecture.prompt.md](./14-event-driven-architecture.prompt.md) once both services build successfully.

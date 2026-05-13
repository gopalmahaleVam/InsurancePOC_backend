---
description: 'Create a BackgroundService hosted service for periodic background processing with graceful shutdown support.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Add Background Services

## Mission
Create a `BackgroundService` hosted service for `${input:serviceName:DataCleanup}` in `${input:projectName:YourProject}` that runs on a configurable interval, uses scoped services safely, and handles graceful shutdown.

## Scope & Preconditions
- Steps 01–14 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Background service name: `${input:serviceName:DataCleanup}`
- Run interval in seconds: `${input:intervalSeconds:3600}`
- If `serviceName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `serviceName` | Background service name | `DataCleanup` |
| `intervalSeconds` | Run interval in seconds | `3600` |

## Workflow

### Step 1 — Create Background Service
Create `src/${input:namespace}.Infrastructure/BackgroundServices/${input:serviceName}Service.cs`:
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ${input:namespace}.Infrastructure.BackgroundServices;

public class ${input:serviceName}Service : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<${input:serviceName}Service> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(${input:intervalSeconds});

    public ${input:serviceName}Service(IServiceProvider serviceProvider, ILogger<${input:serviceName}Service> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{ServiceName} started", nameof(${input:serviceName}Service));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error in {ServiceName}", nameof(${input:serviceName}Service));
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("{ServiceName} stopped", nameof(${input:serviceName}Service));
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        // Resolve scoped services here
        // Example: var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger.LogInformation("{ServiceName} executing work at {Time}", nameof(${input:serviceName}Service), DateTime.UtcNow);
        await Task.CompletedTask;
    }
}
```

### Step 2 — Register Background Service
Update `src/${input:namespace}.Infrastructure/DependencyInjection.cs`:
```csharp
services.AddHostedService<${input:serviceName}Service>();
```

### Step 3 — Add Interval Configuration to appsettings.json
```json
"BackgroundServices": {
  "${input:serviceName}IntervalSeconds": ${input:intervalSeconds}
}
```

### Step 4 — Read Interval from Configuration (Optional)
Update the service to read interval from configuration:
```csharp
var intervalSeconds = configuration.GetValue<int>("BackgroundServices:${input:serviceName}IntervalSeconds", ${input:intervalSeconds});
_interval = TimeSpan.FromSeconds(intervalSeconds);
```

### Step 5 — Verify Build and Run
```bash
dotnet build
dotnet run --project src/${input:namespace}.API
```

Confirm in logs: `${input:serviceName}Service started`

## Output Expectations
- `${input:serviceName}Service.cs` in `Infrastructure/BackgroundServices/`
- Service registered via `AddHostedService<${input:serviceName}Service>()`
- Interval configurable via `appsettings.json`
- Scoped services resolved via `IServiceProvider.CreateScope()`
- `dotnet build` exits with code 0
- Service start message visible in application logs

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] Background service uses `IServiceProvider.CreateScope()` for scoped dependencies
- [ ] `OperationCanceledException` not swallowed — allows graceful shutdown
- [ ] Interval read from configuration — not hardcoded
- [ ] Service logs start and stop messages
- [ ] No singleton-scoped service resolution issues

### Failure Triggers
- Stop if `dotnet build` fails — check `AddHostedService` registration
- Stop if service throws `InvalidOperationException` about scoped services — use `CreateScope()`
- Request `serviceName` from user if not provided

### Next Step
Proceed to [16-api-documentation.prompt.md](./16-api-documentation.prompt.md) once build succeeds and service starts.

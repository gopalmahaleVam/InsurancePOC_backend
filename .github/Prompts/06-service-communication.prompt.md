---
description: 'Generate typed HTTP clients with Polly resilience policies and a domain event bus for inter-service communication.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Generate Service Communication & Integration

## Mission
Add typed HTTP clients with Polly retry/circuit-breaker policies, a `LoggingHandler`, and an in-memory `IEventBus` with domain event handlers to `${input:projectName:YourProject}`.

## Scope & Preconditions
- Steps 01–05 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- External service to integrate: `${input:externalServiceName:PaymentService}`
- If `externalServiceName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `externalServiceName` | Name of the external HTTP service | `PaymentService` |

## Workflow

### Step 1 — Install Polly Package
```bash
dotnet add src/${input:namespace}.Infrastructure package Microsoft.Extensions.Http.Polly
```

### Step 2 — Create External Service Interface
Create `src/${input:namespace}.Application/Common/Interfaces/I${input:externalServiceName}.cs`:
```csharp
namespace ${input:namespace}.Application.Common.Interfaces;

public interface I${input:externalServiceName}
{
    Task<bool> ProcessAsync(string referenceId, CancellationToken cancellationToken = default);
}
```

### Step 3 — Create Typed HTTP Client Implementation
Create `src/${input:namespace}.Infrastructure/Services/${input:externalServiceName}.cs`:
```csharp
using ${input:namespace}.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ${input:namespace}.Infrastructure.Services;

public class ${input:externalServiceName} : I${input:externalServiceName}
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<${input:externalServiceName}> _logger;

    public ${input:externalServiceName}(HttpClient httpClient, ILogger<${input:externalServiceName}> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ProcessAsync(string referenceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/process/{referenceId}", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{ServiceName} unavailable for reference {ReferenceId}", nameof(${input:externalServiceName}), referenceId);
            return false;
        }
    }
}
```

### Step 4 — Create LoggingHandler
Create `src/${input:namespace}.Infrastructure/Services/LoggingHandler.cs`:
```csharp
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ${input:namespace}.Infrastructure.Services;

public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger) => _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid();
        _logger.LogInformation("HTTP {Method} {Uri} [{RequestId}]", request.Method, request.RequestUri, requestId);
        var sw = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        sw.Stop();
        _logger.LogInformation("HTTP {StatusCode} in {ElapsedMs}ms [{RequestId}]", response.StatusCode, sw.ElapsedMilliseconds, requestId);
        return response;
    }
}
```

### Step 5 — Register HTTP Client with Polly in DependencyInjection.cs
Update `src/${input:namespace}.Infrastructure/DependencyInjection.cs`:
```csharp
services.AddTransient<LoggingHandler>();

services.AddHttpClient<I${input:externalServiceName}, ${input:externalServiceName}>(client =>
{
    client.BaseAddress = new Uri(configuration["${input:externalServiceName}:BaseUrl"]
        ?? throw new InvalidOperationException("${input:externalServiceName}:BaseUrl not configured"));
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
.AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)))
.AddHttpMessageHandler<LoggingHandler>();
```

### Step 6 — Add appsettings.json Entry
Add to `src/${input:namespace}.API/appsettings.json`:
```json
"${input:externalServiceName}": {
  "BaseUrl": "https://your-service-url"
}
```

### Step 7 — Verify Build
```bash
dotnet build
```

## Output Expectations
- `I${input:externalServiceName}.cs` in `Application/Common/Interfaces/`
- `${input:externalServiceName}.cs` in `Infrastructure/Services/`
- `LoggingHandler.cs` in `Infrastructure/Services/`
- HTTP client registered with retry (3 attempts, exponential backoff) and circuit breaker (5 failures, 30s break)
- `dotnet build` exits with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] HTTP client interface defined in Application layer (not Infrastructure)
- [ ] Polly retry policy uses exponential backoff
- [ ] Circuit breaker configured with sensible thresholds
- [ ] `LoggingHandler` logs request/response with timing
- [ ] Base URL read from configuration — not hardcoded
- [ ] No API keys or secrets hardcoded in source files

### Failure Triggers
- Stop if `BaseUrl` configuration key is missing — request it from the user
- Stop if `dotnet build` fails after adding Polly packages
- Stop if Infrastructure references API project

### Next Step
Proceed to [07-logging-security-concerns.prompt.md](./07-logging-security-concerns.prompt.md) once build succeeds.

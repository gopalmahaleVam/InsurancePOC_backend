---
description: 'Add health checks for database and external services, and configure OpenTelemetry or Application Insights telemetry.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Add Monitoring & Observability

## Mission
Add SQL Server health check, a custom external service health check, and Application Insights (or OpenTelemetry) telemetry to `${input:projectName:YourProject}`.

## Scope & Preconditions
- Steps 01–16 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Telemetry provider: `${input:telemetryProvider:ApplicationInsights}` (ApplicationInsights / OpenTelemetry)
- If `telemetryProvider` is not provided, default to `ApplicationInsights`

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `telemetryProvider` | Telemetry provider | `ApplicationInsights` |

## Workflow

### Step 1 — Install Health Check Packages
```bash
dotnet add src/${input:namespace}.API package AspNetCore.HealthChecks.SqlServer
dotnet add src/${input:namespace}.API package Microsoft.Extensions.Diagnostics.HealthChecks
```

### Step 2 — Install Telemetry Package
```bash
# For Application Insights:
dotnet add src/${input:namespace}.API package Microsoft.ApplicationInsights.AspNetCore

# For OpenTelemetry (alternative):
# dotnet add src/${input:namespace}.API package OpenTelemetry.Extensions.Hosting
# dotnet add src/${input:namespace}.API package OpenTelemetry.Instrumentation.AspNetCore
```

### Step 3 — Configure Health Checks in Program.cs
```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "database",
        tags: new[] { "db", "sql" })
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"));

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db")
});
```

### Step 4 — Configure Application Insights
```csharp
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});
```

Add to `appsettings.json`:
```json
"ApplicationInsights": {
  "ConnectionString": "<your-connection-string>"
}
```

### Step 5 — Create Custom Health Check
Create `src/${input:namespace}.API/HealthChecks/ExternalServiceHealthCheck.cs`:
```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ${input:namespace}.API.HealthChecks;

public class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _healthUrl;

    public ExternalServiceHealthCheck(IHttpClientFactory factory, IConfiguration configuration)
    {
        _httpClient = factory.CreateClient();
        _healthUrl = configuration["ExternalService:HealthUrl"] ?? string.Empty;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_healthUrl))
            return HealthCheckResult.Degraded("ExternalService:HealthUrl not configured");

        try
        {
            var response = await _httpClient.GetAsync(_healthUrl, cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy($"Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}
```

Register it:
```csharp
builder.Services.AddHealthChecks()
    .AddCheck<ExternalServiceHealthCheck>("external-service", tags: new[] { "external" });
```

### Step 6 — Verify Health Endpoints
```bash
dotnet run --project src/${input:namespace}.API
```

```bash
curl https://localhost:5001/health
curl https://localhost:5001/health/ready
```

Both should return `Healthy`.

## Output Expectations
- `/health` endpoint returns `Healthy` JSON
- `/health/ready` endpoint checks database connectivity
- `ExternalServiceHealthCheck.cs` in `API/HealthChecks/`
- Application Insights telemetry configured
- `dotnet build` exits with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] `GET /health` returns HTTP 200 with `Healthy` status
- [ ] `GET /health/ready` checks database tag
- [ ] Application Insights connection string in configuration — not hardcoded
- [ ] Custom health check handles exceptions gracefully
- [ ] Health check endpoints excluded from authentication middleware

### Failure Triggers
- Stop if `/health` returns non-200 — check SQL Server connection string
- Stop if Application Insights connection string is missing — request it from the user
- Stop if `dotnet build` fails after adding health check packages

### Next Step
Proceed to [18-deployment-strategies.prompt.md](./18-deployment-strategies.prompt.md) once health endpoints return `Healthy`.

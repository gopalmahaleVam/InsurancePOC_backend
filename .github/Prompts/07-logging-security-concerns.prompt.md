---
description: 'Add Serilog structured logging, JWT authentication, correlation ID middleware, and strongly typed configuration.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Add Logging, Security & Cross-Cutting Concerns

## Mission
Configure Serilog with correlation ID enrichment, JWT Bearer authentication, strongly typed `JwtSettings` with validation, and `CorrelationIdMiddleware` for `${input:projectName:YourProject}`.

## Scope & Preconditions
- Steps 01–06 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- JWT secret key minimum length: 32 characters
- If JWT settings are not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `jwtIssuer` | JWT token issuer | `YourProject` |
| `jwtAudience` | JWT token audience | `YourProjectUsers` |

## Workflow

### Step 1 — Install Packages
```bash
dotnet add src/${input:namespace}.API package Serilog.AspNetCore
dotnet add src/${input:namespace}.API package Serilog.Sinks.Console
dotnet add src/${input:namespace}.API package Serilog.Sinks.File
dotnet add src/${input:namespace}.API package Microsoft.AspNetCore.Authentication.JwtBearer
```

### Step 2 — Create Strongly Typed JwtSettings
Create `src/${input:namespace}.API/Configuration/JwtSettings.cs`:
```csharp
namespace ${input:namespace}.API.Configuration;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
```

### Step 3 — Create CorrelationIdMiddleware
Create `src/${input:namespace}.API/Middleware/CorrelationIdMiddleware.cs`:
```csharp
using Serilog.Context;

namespace ${input:namespace}.API.Middleware;

public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
```

### Step 4 — Configure Serilog in Program.cs
Update `src/${input:namespace}.API/Program.cs` to add Serilog before `builder.Build()`:
```csharp
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7));
```

And add middleware after `app.UseMiddleware<ExceptionHandlingMiddleware>()`:
```csharp
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
```

### Step 5 — Configure JWT Authentication in Program.cs
```csharp
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings not configured");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
```

### Step 6 — Add JwtSettings to appsettings.json
```json
"JwtSettings": {
  "SecretKey": "<replace-with-32-char-minimum-secret>",
  "Issuer": "${input:jwtIssuer}",
  "Audience": "${input:jwtAudience}",
  "ExpirationMinutes": 60
}
```

### Step 7 — Verify Build and Run
```bash
dotnet build
dotnet run --project src/${input:namespace}.API
```

## Output Expectations
- `JwtSettings.cs` in `API/Configuration/`
- `CorrelationIdMiddleware.cs` in `API/Middleware/`
- Serilog writing to console and `logs/app-.log`
- JWT authentication configured in `Program.cs`
- `X-Correlation-ID` header present in all responses
- `dotnet build` exits with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] `X-Correlation-ID` header returned in every response
- [ ] Serilog logs include `CorrelationId` property
- [ ] JWT `SecretKey` is at least 32 characters
- [ ] JWT settings read from configuration — not hardcoded
- [ ] `app.UseAuthentication()` called before `app.UseAuthorization()`
- [ ] No secrets committed to source control

### Failure Triggers
- Stop if `JwtSettings:SecretKey` is missing or shorter than 32 characters — request it from the user
- Stop if `dotnet build` fails after adding authentication packages
- Stop if `app.UseAuthentication()` is missing from `Program.cs`

### Next Step
Proceed to [08-testing-quality.prompt.md](./08-testing-quality.prompt.md) once build succeeds and correlation ID appears in logs.

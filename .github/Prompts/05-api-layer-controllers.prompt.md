---
description: 'Generate RESTful API controllers, ExceptionHandlingMiddleware, and ApiResponse models for the API layer.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Generate API Layer Controllers

## Mission
Create the API layer for `${input:projectName:YourProject}` — a thin `${input:entityName:Customer}sController` that dispatches via MediatR, `ExceptionHandlingMiddleware`, and consistent `ApiResponse<T>` response models.

## Scope & Preconditions
- Steps 01–04 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity to generate controller for: `${input:entityName:Customer}`
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Entity to generate controller for | `Customer` |

## Workflow

### Step 1 — Create ApiResponse Models
Create `src/${input:namespace}.API/Models/ApiResponse.cs`:
```csharp
namespace ${input:namespace}.API.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}
```

### Step 2 — Create ExceptionHandlingMiddleware
Create `src/${input:namespace}.API/Middleware/ExceptionHandlingMiddleware.cs`:
```csharp
using FluentValidation;
using ${input:namespace}.Domain.Exceptions;
using System.Text.Json;

namespace ${input:namespace}.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ValidationException vex => (StatusCodes.Status400BadRequest, string.Join("; ", vex.Errors.Select(e => e.ErrorMessage))),
            NotFoundException nfex => (StatusCodes.Status404NotFound, nfex.Message),
            DomainException dex => (StatusCodes.Status400BadRequest, dex.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;
        var response = new { success = false, message, timestamp = DateTime.UtcNow };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
```

### Step 3 — Create Controller
Create `src/${input:namespace}.API/Controllers/${input:entityName}sController.cs`:
```csharp
using ${input:namespace}.Application.${input:entityName}s.Commands;
using ${input:namespace}.Application.${input:entityName}s.Queries;
using ${input:namespace}.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ${input:namespace}.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ${input:entityName}sController : ControllerBase
{
    private readonly IMediator _mediator;

    public ${input:entityName}sController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAll${input:entityName}sQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Get${input:entityName}ByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Create${input:entityName}Command command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Update${input:entityName}Command command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Delete${input:entityName}Command(id), cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound(result.ErrorMessage);
    }
}
```

### Step 4 — Register Middleware in Program.cs
Ensure `Program.cs` includes:
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```
before `app.MapControllers()`.

### Step 5 — Verify Build and Run
```bash
dotnet build
dotnet run --project src/${input:namespace}.API
```

### Step 6 — Verify Swagger
Open `https://localhost:5001/swagger` and confirm all endpoints are listed.

## Output Expectations
- `ApiResponse.cs` in `API/Models/`
- `ExceptionHandlingMiddleware.cs` in `API/Middleware/`
- `${input:entityName}sController.cs` in `API/Controllers/`
- `dotnet build` exits with code 0
- All 5 CRUD endpoints visible in Swagger UI

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] Controller is thin — no business logic, only MediatR dispatch
- [ ] `ExceptionHandlingMiddleware` handles `ValidationException`, `NotFoundException`, `DomainException`
- [ ] `CreatedAtAction` used for POST returning 201
- [ ] `NoContent` (204) returned for DELETE
- [ ] Controller does not reference Domain layer directly
- [ ] Swagger UI shows all endpoints at `/swagger`

### Failure Triggers
- Stop if `dotnet run` fails — check middleware registration order in `Program.cs`
- Stop if controller references repositories or DbContext directly
- Request `entityName` from user if not provided

### Next Step
Proceed to [06-service-communication.prompt.md](./06-service-communication.prompt.md) once API runs and Swagger is accessible.

---
description: 'Generate CQRS commands, queries, handlers, pipeline behaviours, and Result pattern for the Application layer.'
mode: 'agent'
tools: ['edit/editFiles']
---

# Generate CQRS Application Layer

## Mission
Create the Application layer CQRS implementation for `${input:projectName:YourProject}` — commands, queries, MediatR handlers, `ValidationBehaviour`, `LoggingBehaviour`, and the `Result<T>` pattern for entity `${input:entityName:Customer}`.

## Scope & Preconditions
- Steps 01–03 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity to generate CQRS for: `${input:entityName:Customer}`
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Entity to generate CQRS for | `Customer` |

## Workflow

### Step 1 — Create Result Pattern
Create `src/${input:namespace}.Application/Common/Models/Result.cs`:
```csharp
namespace ${input:namespace}.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }

    private Result(bool isSuccess, T? data, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
}
```

### Step 2 — Create ValidationBehaviour
Create `src/${input:namespace}.Application/Common/Behaviours/ValidationBehaviour.cs`:
```csharp
using FluentValidation;
using MediatR;

namespace ${input:namespace}.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

### Step 3 — Create LoggingBehaviour
Create `src/${input:namespace}.Application/Common/Behaviours/LoggingBehaviour.cs`:
```csharp
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ${input:namespace}.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Handling {RequestName}", requestName);
        var response = await next();
        sw.Stop();
        _logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
        return response;
    }
}
```

### Step 4 — Create Create Command + Validator + Handler
Create `src/${input:namespace}.Application/${input:entityName}s/Commands/Create${input:entityName}Command.cs`:
```csharp
using FluentValidation;
using ${input:namespace}.Application.Common.Models;
using ${input:namespace}.Domain.Interfaces;
using MediatR;

namespace ${input:namespace}.Application.${input:entityName}s.Commands;

public record Create${input:entityName}Command(/* entity-specific properties */) : IRequest<Result<${input:entityName}Dto>>;

public class Create${input:entityName}Validator : AbstractValidator<Create${input:entityName}Command>
{
    public Create${input:entityName}Validator()
    {
        // Add rules here
    }
}

public class Create${input:entityName}Handler : IRequestHandler<Create${input:entityName}Command, Result<${input:entityName}Dto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public Create${input:entityName}Handler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<${input:entityName}Dto>> Handle(Create${input:entityName}Command request, CancellationToken cancellationToken)
    {
        // Implement handler logic
        throw new NotImplementedException();
    }
}
```

### Step 5 — Create Update + Delete Commands
Follow the same pattern as Step 4 for `Update${input:entityName}Command` and `Delete${input:entityName}Command`.

### Step 6 — Create Queries
Create `src/${input:namespace}.Application/${input:entityName}s/Queries/Get${input:entityName}Queries.cs`:
```csharp
using ${input:namespace}.Application.Common.Models;
using ${input:namespace}.Domain.Interfaces;
using MediatR;

namespace ${input:namespace}.Application.${input:entityName}s.Queries;

public record Get${input:entityName}ByIdQuery(int Id) : IRequest<Result<${input:entityName}Dto>>;

public class Get${input:entityName}ByIdHandler : IRequestHandler<Get${input:entityName}ByIdQuery, Result<${input:entityName}Dto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public Get${input:entityName}ByIdHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<${input:entityName}Dto>> Handle(Get${input:entityName}ByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.${input:entityName}s.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result<${input:entityName}Dto>.Failure($"${input:entityName} with ID {request.Id} was not found.");
        return Result<${input:entityName}Dto>.Success(new ${input:entityName}Dto(/* map properties */));
    }
}

public record GetAll${input:entityName}sQuery : IRequest<Result<IEnumerable<${input:entityName}Dto>>>;

public class GetAll${input:entityName}sHandler : IRequestHandler<GetAll${input:entityName}sQuery, Result<IEnumerable<${input:entityName}Dto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAll${input:entityName}sHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<${input:entityName}Dto>>> Handle(GetAll${input:entityName}sQuery request, CancellationToken cancellationToken)
    {
        var entities = await _unitOfWork.${input:entityName}s.GetAllAsync(cancellationToken);
        return Result<IEnumerable<${input:entityName}Dto>>.Success(entities.Select(e => new ${input:entityName}Dto(/* map */)));
    }
}
```

### Step 7 — Verify Build
```bash
dotnet build
```

## Output Expectations
- `Result<T>` in `Application/Common/Models/`
- `ValidationBehaviour.cs` and `LoggingBehaviour.cs` in `Application/Common/Behaviours/`
- `Create${input:entityName}Command.cs` with validator and handler in `Application/${input:entityName}s/Commands/`
- `Update${input:entityName}Command.cs` and `Delete${input:entityName}Command.cs` in same folder
- `Get${input:entityName}Queries.cs` in `Application/${input:entityName}s/Queries/`
- `dotnet build` exits with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] Commands use `record` types implementing `IRequest<Result<T>>`
- [ ] Queries use `record` types implementing `IRequest<Result<T>>`
- [ ] Each command/query has a co-located validator and handler
- [ ] `ValidationBehaviour` and `LoggingBehaviour` registered in `DependencyInjection.cs`
- [ ] `Result<T>` used consistently — no raw exceptions returned from handlers
- [ ] All handlers accept `CancellationToken`

### Failure Triggers
- Stop if `dotnet build` fails after adding behaviours
- Stop if Application layer references Infrastructure — violates Clean Architecture
- Request `entityName` from user if not provided

### Next Step
Proceed to [05-api-layer-controllers.prompt.md](./05-api-layer-controllers.prompt.md) once build succeeds.

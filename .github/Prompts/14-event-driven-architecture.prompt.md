---
description: 'Implement domain events raised by aggregate roots and dispatched via MediatR after SaveChanges.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Implement Event-Driven Architecture

## Mission
Add domain events to `${input:entityName:Customer}` aggregate root in `${input:projectName:YourProject}`, dispatch them via MediatR `INotification` after `SaveChangesAsync`, and create the first event handler.

## Scope & Preconditions
- Steps 01–13 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity raising events: `${input:entityName:Customer}`
- First event to implement: `${input:eventName:CustomerCreated}`
- If `entityName` or `eventName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Aggregate root raising events | `Customer` |
| `eventName` | First domain event name | `CustomerCreated` |

## Workflow

### Step 1 — Create IDomainEvent Interface
Create `src/${input:namespace}.Domain/Interfaces/IDomainEvent.cs`:
```csharp
using MediatR;

namespace ${input:namespace}.Domain.Interfaces;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredAt { get; }
}
```

### Step 2 — Update BaseEntity to Raise Domain Events
Update `src/${input:namespace}.Domain/Entities/BaseEntity.cs`:
```csharp
using ${input:namespace}.Domain.Interfaces;

namespace ${input:namespace}.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public void MarkAsDeleted() { IsDeleted = true; UpdatedAt = DateTime.UtcNow; }
    public void UpdateTimestamp() { UpdatedAt = DateTime.UtcNow; }
}
```

### Step 3 — Create Domain Event
Create `src/${input:namespace}.Domain/Events/${input:eventName}Event.cs`:
```csharp
using ${input:namespace}.Domain.Interfaces;

namespace ${input:namespace}.Domain.Events;

public class ${input:eventName}Event : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public int ${input:entityName}Id { get; }
    public string FullName { get; }
    public string Email { get; }

    public ${input:eventName}Event(int ${input:entityName}Id, string fullName, string email)
    {
        ${input:entityName}Id = ${input:entityName}Id;
        FullName = fullName;
        Email = email;
    }
}
```

### Step 4 — Raise Event in Entity Constructor
Update `src/${input:namespace}.Domain/Entities/${input:entityName}.cs` constructor to raise the event:
```csharp
RaiseDomainEvent(new ${input:eventName}Event(Id, FullName, email.Value));
```

### Step 5 — Dispatch Events in ApplicationDbContext
Update `src/${input:namespace}.Infrastructure/Data/ApplicationDbContext.cs`:
```csharp
using MediatR;

public class ApplicationDbContext : DbContext
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var entities = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var @event in events)
            await _mediator.Publish(@event, cancellationToken);
    }
}
```

### Step 6 — Create Event Handler
Create `src/${input:namespace}.Application/${input:entityName}s/EventHandlers/${input:eventName}EventHandler.cs`:
```csharp
using MediatR;
using ${input:namespace}.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ${input:namespace}.Application.${input:entityName}s.EventHandlers;

public class ${input:eventName}EventHandler : INotificationHandler<${input:eventName}Event>
{
    private readonly ILogger<${input:eventName}EventHandler> _logger;

    public ${input:eventName}EventHandler(ILogger<${input:eventName}EventHandler> logger) => _logger = logger;

    public Task Handle(${input:eventName}Event notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("${input:entityName} created: {Id} - {FullName}", notification.${input:entityName}Id, notification.FullName);
        return Task.CompletedTask;
    }
}
```

### Step 7 — Verify Build and Tests
```bash
dotnet build
dotnet test
```

## Output Expectations
- `IDomainEvent.cs` in `Domain/Interfaces/`
- `BaseEntity` updated with `DomainEvents` collection
- `${input:eventName}Event.cs` in `Domain/Events/`
- `ApplicationDbContext.SaveChangesAsync` dispatches events via MediatR
- `${input:eventName}EventHandler.cs` in `Application/${input:entityName}s/EventHandlers/`
- `dotnet build` and `dotnet test` exit with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] `dotnet test` passes with no failures
- [ ] Domain events dispatched **after** `SaveChangesAsync` — not before
- [ ] `ClearDomainEvents()` called before publishing to prevent re-dispatch
- [ ] Event handler is registered automatically via MediatR assembly scan
- [ ] `IDomainEvent` extends `INotification` for MediatR compatibility

### Failure Triggers
- Stop if `dotnet build` fails — check `IMediator` injection in `ApplicationDbContext`
- Stop if `dotnet test` fails — check event handler registration
- Request `entityName` or `eventName` from user if not provided

### Next Step
Proceed to [15-background-services.prompt.md](./15-background-services.prompt.md) once build and tests pass.

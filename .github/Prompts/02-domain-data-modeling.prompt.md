---
description: 'Generate rich domain entities, value objects, enums, DTOs, AutoMapper profiles, and FluentValidation validators.'
mode: 'agent'
tools: ['edit/editFiles']
---

# Generate Domain & Data Models

## Mission
Create the Domain layer and Application-layer DTOs for `${input:projectName:YourProject}` following Domain-Driven Design — rich entities with business logic, immutable value objects, enums, request/response DTOs, AutoMapper profiles, and FluentValidation validators.

## Scope & Preconditions
- Step 01 (Solution Architecture Setup) must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Primary entity to scaffold: `${input:entityName:Customer}` (repeat for each domain entity)
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Primary domain entity to generate | `Customer` |

## Workflow

### Step 1 — Create Base Entity
Create `src/${input:namespace}.Domain/Entities/BaseEntity.cs`:
```csharp
namespace ${input:namespace}.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Step 2 — Create Value Objects
Create `src/${input:namespace}.Domain/ValueObjects/Email.cs`:
```csharp
using System.Text.RegularExpressions;

namespace ${input:namespace}.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email format.", nameof(value));
        Value = value.ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is Email other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
```

### Step 3 — Create Domain Exceptions
Create `src/${input:namespace}.Domain/Exceptions/DomainException.cs`:
```csharp
namespace ${input:namespace}.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"{name} with key '{key}' was not found.") { }
}
```

### Step 4 — Create Domain Entity
Create `src/${input:namespace}.Domain/Entities/${input:entityName}.cs` with:
- Private setters on all properties
- Private parameterless constructor for EF Core
- Business methods that enforce invariants
- Domain events raised via `RaiseDomainEvent()`

### Step 5 — Create Domain Enums
Create `src/${input:namespace}.Domain/Enums/Enums.cs` with enums relevant to `${input:entityName}`.

### Step 6 — Create DTOs
Create `src/${input:namespace}.Application/${input:entityName}s/Commands/${input:entityName}Dto.cs`:
```csharp
namespace ${input:namespace}.Application.${input:entityName}s.Commands;

public record ${input:entityName}Dto(int Id, /* entity-specific properties */ DateTime CreatedAt);
```

### Step 7 — Create AutoMapper Profile
Create `src/${input:namespace}.Application/Common/Mappings/${input:entityName}MappingProfile.cs`:
```csharp
using AutoMapper;
using ${input:namespace}.Domain.Entities;

namespace ${input:namespace}.Application.Common.Mappings;

public class ${input:entityName}MappingProfile : Profile
{
    public ${input:entityName}MappingProfile()
    {
        CreateMap<${input:entityName}, ${input:entityName}Dto>();
    }
}
```

### Step 8 — Create FluentValidation Validator
Create validator alongside the command (see Step 04 prompt for command structure).

### Step 9 — Verify Build
```bash
dotnet build
```

## Output Expectations
- `BaseEntity.cs` in `Domain/Entities/`
- `Email.cs` (and other value objects) in `Domain/ValueObjects/`
- `DomainException.cs` and `NotFoundException.cs` in `Domain/Exceptions/`
- `${input:entityName}.cs` entity in `Domain/Entities/`
- Enums in `Domain/Enums/`
- `${input:entityName}Dto.cs` in `Application/${input:entityName}s/`
- AutoMapper profile in `Application/Common/Mappings/`
- `dotnet build` exits with code 0

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] Domain entities have private setters and private EF Core constructor
- [ ] Value objects are immutable and self-validating
- [ ] `DomainException` and `NotFoundException` exist in `Domain/Exceptions/`
- [ ] DTOs use `record` types with `init` properties
- [ ] AutoMapper profile maps all entity properties to DTO
- [ ] No external dependencies in Domain project

### Failure Triggers
- Stop if `dotnet build` fails after entity creation
- Stop if Domain project references Application or Infrastructure — this violates Clean Architecture
- Request `entityName` from user if not provided

### Next Step
Proceed to [03-repository-data-access.prompt.md](./03-repository-data-access.prompt.md) once build succeeds.

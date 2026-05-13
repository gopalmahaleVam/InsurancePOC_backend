---
description: 'Identify and fix code smells, enforce architecture rules, and modernize legacy patterns across the codebase.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal', 'search/usages', 'read/problems']
---

# Automate Code Refactoring

## Mission
Analyze `${input:projectName:YourProject}` for code smells and architecture violations, apply targeted refactorings (extract method, replace magic numbers, remove duplication), and verify all tests still pass.

## Scope & Preconditions
- All 20 previous steps must be completed and `dotnet test` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Refactoring scope: `${input:refactoringScope:all}` (all / domain / application / infrastructure / api)
- Do NOT refactor auto-generated files (`*.g.cs`, `Migrations/`)
- If `refactoringScope` is not provided, default to `all`

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `refactoringScope` | Which layer to refactor | `all` |

## Workflow

### Step 1 — Run Static Analysis
```bash
dotnet build 2>&1 | grep -i "warning\|error"
```

Review all warnings. Prioritize:
- `CS8600`–`CS8625` — nullable reference warnings
- `CA1822` — methods that can be static
- `IDE0` — style violations

### Step 2 — Check Architecture Rules
```bash
dotnet test --filter "FullyQualifiedName~ArchitectureTests"
```

All architecture tests must pass before proceeding.

### Step 3 — Identify Code Smells
Search for and fix the following patterns:

**Magic numbers** — replace with named constants:
```csharp
// Before
if (elapsed > 500) { ... }

// After
private const int SlowRequestThresholdMs = 500;
if (elapsed > SlowRequestThresholdMs) { ... }
```

**Long methods (>20 lines)** — extract focused private methods:
```csharp
// Before: one large Handle() method
// After: Handle() calls ValidateAsync(), CreateEntityAsync(), SaveAsync()
```

**Duplicate null checks** — use `ArgumentNullException.ThrowIfNull()`:
```csharp
// Before
if (param == null) throw new ArgumentNullException(nameof(param));

// After
ArgumentNullException.ThrowIfNull(param);
```

**Sync-over-async** — remove `.Result` and `.Wait()`:
```csharp
// Before
var result = someTask.Result;

// After
var result = await someTask;
```

**Missing file-scoped namespaces** — convert to file-scoped:
```csharp
// Before
namespace MyApp.Domain.Entities { public class Customer { } }

// After
namespace MyApp.Domain.Entities;
public class Customer { }
```

### Step 4 — Apply Refactorings
For each identified issue:
1. Make the targeted change
2. Run `dotnet build` to confirm no new errors
3. Run `dotnet test` to confirm no regressions

### Step 5 — Final Verification
```bash
dotnet build
dotnet test
```

Both must exit with code 0.

### Step 6 — Review Build Warnings
```bash
dotnet build 2>&1 | grep "warning" | wc -l
```

Target: zero warnings (or fewer than before refactoring).

## Output Expectations
- All magic numbers replaced with named constants
- All methods under 20 lines (or documented exceptions)
- No `.Result` or `.Wait()` in async code
- File-scoped namespaces used throughout
- `dotnet build` exits with code 0
- `dotnet test` exits with code 0
- Architecture tests all pass

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] `dotnet test` passes with no failures
- [ ] Architecture tests pass (layer dependency rules enforced)
- [ ] No `.Result` or `.Wait()` in production code
- [ ] No magic numbers — all replaced with named constants
- [ ] File-scoped namespaces used in all `.cs` files
- [ ] `ArgumentNullException.ThrowIfNull()` used for null guards
- [ ] No auto-generated files modified (`Migrations/`, `*.g.cs`)

### Failure Triggers
- Stop if `dotnet test` fails after any refactoring — revert the last change and investigate
- Stop if architecture tests fail — a layer violation was introduced during refactoring
- Do not refactor files in `Migrations/` folder — these are auto-generated

### Completion
All 21 steps are complete. The solution now follows Clean Architecture + CQRS + DDD with full test coverage, CI/CD, monitoring, and code quality enforcement.

Reference the [golden template](../../golden-template/InsuranceApp/) for a complete working example of all patterns.

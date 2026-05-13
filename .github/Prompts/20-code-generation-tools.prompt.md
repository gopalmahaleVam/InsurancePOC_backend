---
description: 'Create a dotnet CLI tool or PowerShell script to scaffold a new feature following the Clean Architecture pattern.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Create Code Generation Tools

## Mission
Create a PowerShell scaffolding script `New-Feature.ps1` for `${input:projectName:YourProject}` that generates all boilerplate files for a new feature (entity, repository interface, commands, queries, controller) from a single command.

## Scope & Preconditions
- Steps 01–19 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Script output path: `${input:scriptsPath:./scripts}`
- If `namespace` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `scriptsPath` | Where to create the script | `./scripts` |

## Workflow

### Step 1 — Create Scripts Directory
```bash
mkdir scripts
```

### Step 2 — Create New-Feature.ps1
Create `scripts/New-Feature.ps1`:
```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$EntityName,

    [Parameter(Mandatory=$false)]
    [string]$Namespace = "${input:namespace}",

    [Parameter(Mandatory=$false)]
    [string]$SolutionRoot = "."
)

$srcPath = "$SolutionRoot/src"

function Write-FileIfNotExists($path, $content) {
    if (Test-Path $path) {
        Write-Host "SKIP (exists): $path" -ForegroundColor Yellow
    } else {
        New-Item -ItemType File -Path $path -Force | Out-Null
        Set-Content -Path $path -Value $content
        Write-Host "CREATED: $path" -ForegroundColor Green
    }
}

# Domain Entity
Write-FileIfNotExists `
    "$srcPath/$Namespace.Domain/Entities/$EntityName.cs" `
    "namespace $Namespace.Domain.Entities;`n`npublic class $EntityName : BaseEntity`n{`n    private $EntityName() { }`n`n    public $EntityName(/* args */)`n    {`n        // Initialize`n    }`n}"

# Repository Interface
Write-FileIfNotExists `
    "$srcPath/$Namespace.Domain/Interfaces/I${EntityName}Repository.cs" `
    "namespace $Namespace.Domain.Interfaces;`n`npublic interface I${EntityName}Repository : IRepository<$EntityName>`n{`n}"

# Create Command
Write-FileIfNotExists `
    "$srcPath/$Namespace.Application/${EntityName}s/Commands/Create${EntityName}Command.cs" `
    "using $Namespace.Application.Common.Models;`nusing $Namespace.Domain.Interfaces;`nusing MediatR;`n`nnamespace $Namespace.Application.${EntityName}s.Commands;`n`npublic record Create${EntityName}Command() : IRequest<Result<${EntityName}Dto>>;`n`npublic record ${EntityName}Dto(int Id);`n`npublic class Create${EntityName}Handler : IRequestHandler<Create${EntityName}Command, Result<${EntityName}Dto>>`n{`n    private readonly IUnitOfWork _unitOfWork;`n    public Create${EntityName}Handler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;`n    public Task<Result<${EntityName}Dto>> Handle(Create${EntityName}Command request, CancellationToken cancellationToken)`n        => throw new NotImplementedException();`n}"

# Controller
Write-FileIfNotExists `
    "$srcPath/$Namespace.API/Controllers/${EntityName}sController.cs" `
    "using MediatR;`nusing Microsoft.AspNetCore.Mvc;`nusing $Namespace.Application.${EntityName}s.Commands;`n`nnamespace $Namespace.API.Controllers;`n`n[ApiController]`n[Route(`"api/[controller]`")]`npublic class ${EntityName}sController : ControllerBase`n{`n    private readonly IMediator _mediator;`n    public ${EntityName}sController(IMediator mediator) => _mediator = mediator;`n`n    [HttpPost]`n    public async Task<IActionResult> Create([FromBody] Create${EntityName}Command command, CancellationToken ct)`n    {`n        var result = await _mediator.Send(command, ct);`n        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);`n    }`n}"

Write-Host "`nScaffolding complete for '$EntityName'. Run 'dotnet build' to verify." -ForegroundColor Cyan
```

### Step 3 — Test the Script
```powershell
./scripts/New-Feature.ps1 -EntityName "Policy" -Namespace "${input:namespace}"
```

### Step 4 — Verify Generated Files Build
```bash
dotnet build
```

## Output Expectations
- `scripts/New-Feature.ps1` created
- Running the script generates: entity, repository interface, command+handler, controller
- `Write-FileIfNotExists` prevents overwriting existing files
- `dotnet build` exits with code 0 after scaffolding

## Quality Assurance

### Validation Checklist
- [ ] Script runs without errors on PowerShell 7+
- [ ] Generated files compile with `dotnet build`
- [ ] Script is idempotent — skips existing files
- [ ] Generated entity inherits `BaseEntity`
- [ ] Generated controller dispatches via MediatR
- [ ] Script accepts `EntityName` as required parameter

### Failure Triggers
- Stop if `dotnet build` fails after scaffolding — fix generated file templates
- Stop if script throws errors — check PowerShell version compatibility
- Request `namespace` from user if not provided

### Next Step
Proceed to [21-code-refactoring-automation.prompt.md](./21-code-refactoring-automation.prompt.md) once scaffolding script works and generated code builds.

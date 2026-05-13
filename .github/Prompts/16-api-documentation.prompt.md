---
description: 'Configure Swagger/OpenAPI with JWT Bearer authentication, XML comments, and enum string schema for the API.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Configure API Documentation

## Mission
Configure Swagger/OpenAPI for `${input:projectName:YourProject}` with JWT Bearer authentication support, XML comment documentation, enum string display, and a custom API title and description.

## Scope & Preconditions
- Steps 01–15 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- API title: `${input:apiTitle:YourProject API}`
- API version: `${input:apiVersion:v1}`
- If `apiTitle` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `apiTitle` | Swagger UI title | `YourProject API` |
| `apiVersion` | API version string | `v1` |

## Workflow

### Step 1 — Enable XML Documentation in API Project
Update `src/${input:namespace}.API/${input:namespace}.API.csproj`:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

### Step 2 — Configure Swagger in Program.cs
Replace the default `AddSwaggerGen()` call with:
```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("${input:apiVersion}", new OpenApiInfo
    {
        Title = "${input:apiTitle}",
        Version = "${input:apiVersion}",
        Description = "API for ${input:projectName}"
    });

    // JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Enter: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    // Display enums as strings
    options.UseInlineDefinitionsForEnums();
});
```

### Step 3 — Add XML Comments to Controller Actions
Add XML doc comments to all controller actions. Example:
```csharp
/// <summary>
/// Retrieves all ${input:entityName}s.
/// </summary>
/// <returns>List of ${input:entityName}s</returns>
/// <response code="200">Returns the list</response>
[HttpGet]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> GetAll(CancellationToken cancellationToken) { ... }
```

### Step 4 — Verify Swagger UI
```bash
dotnet run --project src/${input:namespace}.API
```

Open `https://localhost:5001/swagger` and confirm:
- Title shows `${input:apiTitle}`
- Authorize button present (JWT Bearer)
- All endpoints listed with descriptions

## Output Expectations
- XML documentation enabled in `.csproj`
- Swagger configured with JWT Bearer security definition
- XML comments included in Swagger UI
- `https://localhost:5001/swagger` accessible
- All controller actions have XML doc comments

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] Swagger UI accessible at `/swagger`
- [ ] JWT Bearer "Authorize" button visible in Swagger UI
- [ ] All endpoints listed with descriptions from XML comments
- [ ] XML documentation file generated in build output
- [ ] No `CS1591` warnings (missing XML comments) breaking the build

### Failure Triggers
- Stop if Swagger UI is not accessible — check `UseSwagger()` and `UseSwaggerUI()` in `Program.cs`
- Stop if XML file not found — verify `GenerateDocumentationFile` is set in `.csproj`
- Request `apiTitle` from user if not provided

### Next Step
Proceed to [17-monitoring-observability.prompt.md](./17-monitoring-observability.prompt.md) once Swagger UI is accessible with all endpoints documented.

---
description: 'Add .editorconfig, Directory.Build.props with analyzers, and StyleCop configuration to enforce code quality standards.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Enforce Code Quality Standards

## Mission
Configure `.editorconfig`, `Directory.Build.props` with Roslyn analyzers, and `stylecop.json` for `${input:projectName:YourProject}` to enforce consistent formatting, naming conventions, and static analysis across all projects.

## Scope & Preconditions
- Steps 01–08 must be completed and `dotnet test` must pass
- Company name for copyright headers: `${input:companyName:YourCompany}`
- Files are created at the solution root
- If `companyName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `companyName` | Company name for copyright | `YourCompany` |
| `namespace` | Root namespace | `YourCompany.YourProject` |

## Workflow

### Step 1 — Create .editorconfig at Solution Root
Create `.editorconfig`:
```ini
root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true

[*.cs]
indent_style = space
indent_size = 4
dotnet_sort_system_directives_first = true
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
csharp_indent_case_contents = true
csharp_indent_switch_labels = true
csharp_space_after_keywords_in_control_flow_statements = true
csharp_prefer_simple_using_statement = true:suggestion
csharp_style_namespace_declarations = file_scoped:warning
dotnet_naming_rule.interfaces_prefixed_i.severity = warning
dotnet_naming_rule.interfaces_prefixed_i.symbols = interface
dotnet_naming_rule.interfaces_prefixed_i.style = prefix_i
dotnet_naming_symbols.interface.applicable_kinds = interface
dotnet_naming_style.prefix_i.required_prefix = I
dotnet_naming_style.prefix_i.capitalization = pascal_case

[*.{json,yml,yaml}]
indent_style = space
indent_size = 2
```

### Step 2 — Create Directory.Build.props at Solution Root
Create `Directory.Build.props`:
```xml
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisLevel>latest</AnalysisLevel>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <WarningsNotAsErrors>CS1591</WarningsNotAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.507">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <AdditionalFiles Include="$(MSBuildThisFileDirectory)stylecop.json" />
  </ItemGroup>
</Project>
```

### Step 3 — Create stylecop.json at Solution Root
Create `stylecop.json`:
```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json",
  "settings": {
    "documentationRules": {
      "companyName": "${input:companyName}",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.",
      "xmlHeader": false
    },
    "orderingRules": {
      "usingDirectivesPlacement": "outsideNamespace"
    },
    "namingRules": {
      "allowCommonHungarianPrefixes": false
    },
    "layoutRules": {
      "newlineAtEndOfFile": "require",
      "allowConsecutiveUsings": true
    }
  }
}
```

### Step 4 — Verify Build with Analyzers
```bash
dotnet build
```

### Step 5 — Run Tests to Confirm No Regressions
```bash
dotnet test
```

## Output Expectations
- `.editorconfig` at solution root
- `Directory.Build.props` at solution root
- `stylecop.json` at solution root
- `dotnet build` exits with code 0
- `dotnet test` exits with code 0
- File-scoped namespaces enforced across all `.cs` files

## Quality Assurance

### Validation Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] `dotnet test` passes with no failures
- [ ] `.editorconfig` enforces 4-space indentation for C#
- [ ] `Directory.Build.props` applies to all projects automatically
- [ ] `stylecop.json` references correct company name
- [ ] File-scoped namespace style enforced (`namespace X;` not `namespace X { }`)
- [ ] No new warnings introduced by analyzers that break the build

### Failure Triggers
- Stop if `dotnet build` introduces new errors from analyzers — fix violations before proceeding
- Stop if `dotnet test` fails after adding analyzers — check for test project conflicts
- Request `companyName` from user if not provided

### Next Step
Proceed to [10-unit-test-creation.prompt.md](./10-unit-test-creation.prompt.md) once build and tests pass cleanly.

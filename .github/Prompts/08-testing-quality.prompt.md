---
description: 'Generate xUnit unit tests, integration tests with WebApplicationFactory, and architecture tests with NetArchTest.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Generate Tests & Quality Assurance

## Mission
Create comprehensive unit tests for domain entities and CQRS handlers, integration tests using `WebApplicationFactory` with in-memory database, and architecture tests enforcing Clean Architecture layer rules for `${input:projectName:YourProject}`.

## Scope & Preconditions
- Steps 01–07 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity under test: `${input:entityName:Customer}`
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Entity to generate tests for | `Customer` |

## Workflow

### Step 1 — Install Test Packages
```bash
dotnet add tests/${input:namespace}.UnitTests package Moq
dotnet add tests/${input:namespace}.UnitTests package FluentAssertions
dotnet add tests/${input:namespace}.UnitTests package Microsoft.EntityFrameworkCore.InMemory
dotnet add tests/${input:namespace}.ArchitectureTests package NetArchTest.Rules
```

### Step 2 — Create Domain Entity Tests
Create `tests/${input:namespace}.UnitTests/${input:entityName}s/${input:entityName}Tests.cs`:
```csharp
using FluentAssertions;
using ${input:namespace}.Domain.Entities;
using ${input:namespace}.Domain.Exceptions;
using ${input:namespace}.Domain.ValueObjects;
using Xunit;

namespace ${input:namespace}.UnitTests.${input:entityName}s;

public class ${input:entityName}Tests
{
    [Fact]
    public void Constructor_WithValidData_Creates${input:entityName}()
    {
        // Arrange & Act
        var entity = new ${input:entityName}(/* valid args */);

        // Assert
        entity.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        // Act
        var act = () => new ${input:entityName}(invalidName /* other args */);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
```

### Step 3 — Create Handler Tests
Create `tests/${input:namespace}.UnitTests/${input:entityName}s/Create${input:entityName}HandlerTests.cs`:
```csharp
using FluentAssertions;
using Moq;
using ${input:namespace}.Application.${input:entityName}s.Commands;
using ${input:namespace}.Domain.Interfaces;
using Xunit;

namespace ${input:namespace}.UnitTests.${input:entityName}s;

public class Create${input:entityName}HandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<I${input:entityName}Repository> _repoMock = new();

    public Create${input:entityName}HandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.${input:entityName}s).Returns(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsSuccess()
    {
        _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(false);

        var handler = new Create${input:entityName}Handler(_unitOfWorkMock.Object);
        var command = new Create${input:entityName}Command(/* valid args */);

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDuplicate_ReturnsFailure()
    {
        _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(true);

        var handler = new Create${input:entityName}Handler(_unitOfWorkMock.Object);
        var command = new Create${input:entityName}Command(/* valid args */);

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("already exists");
    }
}
```

### Step 4 — Create Architecture Tests
Create `tests/${input:namespace}.ArchitectureTests/ArchitectureTests.cs`:
```csharp
using NetArchTest.Rules;
using Xunit;

namespace ${input:namespace}.ArchitectureTests;

public class ArchitectureTests
{
    private const string DomainNs = "${input:namespace}.Domain";
    private const string ApplicationNs = "${input:namespace}.Application";
    private const string InfrastructureNs = "${input:namespace}.Infrastructure";
    private const string ApiNs = "${input:namespace}.API";

    [Fact]
    public void Domain_ShouldNot_DependOn_Application()
    {
        var result = Types.InNamespace(DomainNs).ShouldNot().HaveDependencyOn(ApplicationNs).GetResult();
        Assert.True(result.IsSuccessful, "Domain must not reference Application.");
    }

    [Fact]
    public void Domain_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InNamespace(DomainNs).ShouldNot().HaveDependencyOn(InfrastructureNs).GetResult();
        Assert.True(result.IsSuccessful, "Domain must not reference Infrastructure.");
    }

    [Fact]
    public void Application_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InNamespace(ApplicationNs).ShouldNot().HaveDependencyOn(InfrastructureNs).GetResult();
        Assert.True(result.IsSuccessful, "Application must not reference Infrastructure.");
    }

    [Fact]
    public void Infrastructure_ShouldNot_DependOn_API()
    {
        var result = Types.InNamespace(InfrastructureNs).ShouldNot().HaveDependencyOn(ApiNs).GetResult();
        Assert.True(result.IsSuccessful, "Infrastructure must not reference API.");
    }

    [Fact]
    public void Controllers_ShouldNot_DependOn_Domain_Directly()
    {
        var result = Types.InNamespace($"{ApiNs}.Controllers").ShouldNot().HaveDependencyOn(DomainNs).GetResult();
        Assert.True(result.IsSuccessful, "Controllers must not reference Domain directly.");
    }
}
```

### Step 5 — Run All Tests
```bash
dotnet test
```

## Output Expectations
- `${input:entityName}Tests.cs` in `UnitTests/${input:entityName}s/`
- `Create${input:entityName}HandlerTests.cs` in `UnitTests/${input:entityName}s/`
- `ArchitectureTests.cs` in `ArchitectureTests/`
- All tests pass: `dotnet test` exits with code 0
- Architecture tests enforce all 5 layer dependency rules

## Quality Assurance

### Validation Checklist
- [ ] `dotnet test` passes with no failures
- [ ] Domain entity tests cover constructor validation and business methods
- [ ] Handler tests cover success and duplicate/failure scenarios
- [ ] Architecture tests enforce all Clean Architecture layer rules
- [ ] Tests follow AAA pattern (no Arrange/Act/Assert comments needed)
- [ ] No test depends on another test's state
- [ ] Mocks used only for external dependencies (repositories, services)

### Failure Triggers
- Stop if `dotnet test` fails — fix failing tests before proceeding
- Stop if architecture tests fail — this indicates a layer violation that must be fixed
- Request `entityName` from user if not provided

### Next Step
Proceed to [09-code-quality-standards.prompt.md](./09-code-quality-standards.prompt.md) once all tests pass.

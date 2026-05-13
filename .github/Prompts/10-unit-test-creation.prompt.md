---
description: 'Generate comprehensive xUnit unit tests with Moq and FluentAssertions for domain entities, handlers, and validators.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Generate Unit Tests

## Mission
Write comprehensive unit tests for `${input:entityName:Customer}` domain entity, CQRS handlers, and FluentValidation validators in `${input:projectName:YourProject}` using xUnit, Moq, and FluentAssertions following the AAA pattern.

## Scope & Preconditions
- Steps 01–09 must be completed and `dotnet build` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Entity under test: `${input:entityName:Customer}`
- Target code coverage: 80%+
- If `entityName` is not provided, stop and ask the user

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `entityName` | Entity to generate tests for | `Customer` |

## Workflow

### Step 1 — Create Entity Tests
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
    public void Constructor_WithValidData_Creates${input:entityName}WithActiveStatus()
    {
        var entity = new ${input:entityName}(/* valid args */);

        entity.Should().NotBeNull();
        // Assert expected initial state
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        var act = () => new ${input:entityName}(invalidName /* other args */);

        act.Should().Throw<ArgumentException>().WithMessage("*name*");
    }

    [Fact]
    public void Deactivate_WhenActive_SetsStatusToInactive()
    {
        var entity = new ${input:entityName}(/* valid args */);

        entity.Deactivate();

        // Assert deactivated state
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsDomainException()
    {
        var entity = new ${input:entityName}(/* valid args */);
        entity.Deactivate();

        var act = () => entity.Deactivate();

        act.Should().Throw<DomainException>();
    }
}
```

### Step 2 — Create Value Object Tests
Create `tests/${input:namespace}.UnitTests/ValueObjects/EmailTests.cs`:
```csharp
using FluentAssertions;
using ${input:namespace}.Domain.ValueObjects;
using Xunit;

namespace ${input:namespace}.UnitTests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("USER@EXAMPLE.COM")]
    public void Constructor_WithValidEmail_NormalizesToLowercase(string input)
    {
        var email = new Email(input);

        email.Value.Should().Be(input.ToLowerInvariant());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    public void Constructor_WithInvalidEmail_ThrowsArgumentException(string invalid)
    {
        var act = () => new Email(invalid);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_WithSameValue_ReturnsTrue()
    {
        var email1 = new Email("test@example.com");
        var email2 = new Email("TEST@EXAMPLE.COM");

        email1.Should().Be(email2);
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
    public async Task Handle_WhenEmailIsUnique_ReturnsSuccess()
    {
        _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(false);
        var handler = new Create${input:entityName}Handler(_unitOfWorkMock.Object);
        var command = new Create${input:entityName}Command(/* valid args */);

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsFailure()
    {
        _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(true);
        var handler = new Create${input:entityName}Handler(_unitOfWorkMock.Object);
        var command = new Create${input:entityName}Command(/* valid args */);

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("already exists");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
```

### Step 4 — Create Validator Tests
Create `tests/${input:namespace}.UnitTests/${input:entityName}s/Create${input:entityName}ValidatorTests.cs`:
```csharp
using FluentAssertions;
using ${input:namespace}.Application.${input:entityName}s.Commands;
using Xunit;

namespace ${input:namespace}.UnitTests.${input:entityName}s;

public class Create${input:entityName}ValidatorTests
{
    private readonly Create${input:entityName}Validator _validator = new();

    [Fact]
    public async Task Validate_WithValidCommand_PassesValidation()
    {
        var command = new Create${input:entityName}Command(/* valid args */);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_WithMissingRequiredField_FailsValidation(string invalidValue)
    {
        var command = new Create${input:entityName}Command(invalidValue /* other args */);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}
```

### Step 5 — Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## Output Expectations
- `${input:entityName}Tests.cs` in `UnitTests/${input:entityName}s/`
- `EmailTests.cs` in `UnitTests/ValueObjects/`
- `Create${input:entityName}HandlerTests.cs` in `UnitTests/${input:entityName}s/`
- `Create${input:entityName}ValidatorTests.cs` in `UnitTests/${input:entityName}s/`
- All tests pass: `dotnet test` exits with code 0
- Coverage report generated in `./coverage/`

## Quality Assurance

### Validation Checklist
- [ ] `dotnet test` passes with no failures
- [ ] Entity tests cover constructor validation and all business methods
- [ ] Handler tests cover success, duplicate, and error scenarios
- [ ] Validator tests cover valid and invalid inputs
- [ ] Value object tests cover equality and normalization
- [ ] No test depends on another test's state or order
- [ ] Mocks used only for `IUnitOfWork` and repositories — not domain code
- [ ] Coverage report generated successfully

### Failure Triggers
- Stop if `dotnet test` fails — fix failing tests before proceeding
- Request `entityName` from user if not provided

### Next Step
Proceed to [11-integration-testing.prompt.md](./11-integration-testing.prompt.md) once all unit tests pass.

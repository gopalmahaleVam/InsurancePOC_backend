using FluentValidation;
using Insurance.Application.Features.Users.Commands;
using Insurance.Application.Features.Users.Queries;

namespace Insurance.Application.Features.Users.Validators;

/// <summary>
/// Validator for CreateUserCommand.
/// Ensures all required fields are present and valid before user creation.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        // Username validation
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Username can only contain letters, numbers, hyphens, and underscores");

        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters");

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one digit")
            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character");

        // First name validation
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .Length(1, 100)
            .WithMessage("First name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        // Last name validation
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .Length(1, 100)
            .WithMessage("Last name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        // Phone number validation (optional but if provided must be valid)
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[\d\s\-\+\(\)]+$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number format is invalid");

        // Role validation
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role must be a valid user role");
    }
}

/// <summary>
/// Validator for UpdateUserCommand.
/// Validates only the fields that are being updated.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        // ID validation
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        // First name validation (optional)
        RuleFor(x => x.FirstName)
            .Length(1, 100)
            .When(x => x.FirstName is not null)
            .WithMessage("First name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
            .When(x => x.FirstName is not null)
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        // Last name validation (optional)
        RuleFor(x => x.LastName)
            .Length(1, 100)
            .When(x => x.LastName is not null)
            .WithMessage("Last name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
            .When(x => x.LastName is not null)
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        // Email validation (optional)
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => x.Email is not null)
            .WithMessage("Email must be a valid email address")
            .MaximumLength(255)
            .When(x => x.Email is not null)
            .WithMessage("Email must not exceed 255 characters");

        // Phone number validation (optional)
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[\d\s\-\+\(\)]+$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number format is invalid");

        // Role validation (optional)
        RuleFor(x => x.Role)
            .IsInEnum()
            .When(x => x.Role.HasValue)
            .WithMessage("Role must be a valid user role");
    }
}

/// <summary>
/// Validator for DeleteUserCommand.
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

/// <summary>
/// Validator for GetUserByIdQuery.
/// </summary>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

/// <summary>
/// Validator for GetAllUsersQuery.
/// </summary>
public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");
    }
}

/// <summary>
/// Validator for SearchUsersByNameQuery.
/// </summary>
public class SearchUsersByNameQueryValidator : AbstractValidator<SearchUsersByNameQuery>
{
    public SearchUsersByNameQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .WithMessage("Search term is required")
            .MinimumLength(2)
            .WithMessage("Search term must be at least 2 characters");
    }
}

/// <summary>
/// Validator for CheckUsernameAvailabilityQuery.
/// </summary>
public class CheckUsernameAvailabilityQueryValidator : AbstractValidator<CheckUsernameAvailabilityQuery>
{
    public CheckUsernameAvailabilityQueryValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters");
    }
}

/// <summary>
/// Validator for CheckEmailAvailabilityQuery.
/// </summary>
public class CheckEmailAvailabilityQueryValidator : AbstractValidator<CheckEmailAvailabilityQuery>
{
    public CheckEmailAvailabilityQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address");
    }
}

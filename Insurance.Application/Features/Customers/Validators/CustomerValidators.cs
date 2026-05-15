using FluentValidation;
using Insurance.Application.Features.Customers.Commands;
using Insurance.Application.Features.Customers.Queries;

namespace Insurance.Application.Features.Customers.Validators;

/// <summary>
/// Validators for customer-related commands and queries using FluentValidation.
/// Ensures all input data meets business requirements before processing.
/// </summary>

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(1, 100).WithMessage("First name must be between 1 and 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^[0-9\-\+\(\)\s]+$").WithMessage("Phone number contains invalid characters")
            .Length(10, 20).WithMessage("Phone number must be between 10 and 20 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(255).WithMessage("Address cannot exceed 255 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .Length(2, 2).WithMessage("State must be a 2-character state code (e.g., 'CA')");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Zip code must be valid (format: XXXXX or XXXXX-XXXX)");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.UtcNow.AddYears(-150)).WithMessage("Date of birth must be reasonable");
    }
}

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Customer ID must be greater than 0");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(1, 100).WithMessage("First name must be between 1 and 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^[0-9\-\+\(\)\s]+$").WithMessage("Phone number contains invalid characters")
            .Length(10, 20).WithMessage("Phone number must be between 10 and 20 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(255).WithMessage("Address cannot exceed 255 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .Length(2, 2).WithMessage("State must be a 2-character state code (e.g., 'CA')");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Zip code must be valid (format: XXXXX or XXXXX-XXXX)");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.UtcNow.AddYears(-150)).WithMessage("Date of birth must be reasonable");
    }
}

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Customer ID must be greater than 0");
    }
}

public class GetAllCustomersQueryValidator : AbstractValidator<GetAllCustomersQuery>
{
    public GetAllCustomersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");
    }
}

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Customer ID must be greater than 0");
    }
}

public class SearchCustomersByNameQueryValidator : AbstractValidator<SearchCustomersByNameQuery>
{
    public SearchCustomersByNameQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Search term is required")
            .MinimumLength(2).WithMessage("Search term must be at least 2 characters")
            .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters");
    }
}

public class GetCustomersByCityQueryValidator : AbstractValidator<GetCustomersByCityQuery>
{
    public GetCustomersByCityQueryValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");
    }
}

public class GetCustomersByStateQueryValidator : AbstractValidator<GetCustomersByStateQuery>
{
    public GetCustomersByStateQueryValidator()
    {
        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .Length(2, 2).WithMessage("State must be a 2-character state code");
    }
}

public class GetCustomersByZipCodeQueryValidator : AbstractValidator<GetCustomersByZipCodeQuery>
{
    public GetCustomersByZipCodeQueryValidator()
    {
        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Zip code must be valid (format: XXXXX or XXXXX-XXXX)");
    }
}

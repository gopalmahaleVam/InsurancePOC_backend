using Insurance.Application.Common.Models;
using Insurance.Application.Features.Customers.DTOs;
using MediatR;

namespace Insurance.Application.Features.Customers.Commands;

/// <summary>
/// Command to create a new customer.
/// </summary>
public class CreateCustomerCommand : IRequest<Result<CustomerResponseDto>>
{
    /// <summary>
    /// Customer's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Customer's last name.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Customer's email address (must be unique).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Customer's phone number.
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Customer's street address.
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Customer's city.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Customer's state.
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    /// Customer's zip code.
    /// </summary>
    public required string ZipCode { get; set; }

    /// <summary>
    /// Customer's date of birth.
    /// </summary>
    public required DateTime DateOfBirth { get; set; }
}

/// <summary>
/// Command to update an existing customer.
/// </summary>
public class UpdateCustomerCommand : IRequest<Result<CustomerResponseDto>>
{
    /// <summary>
    /// The unique identifier of the customer to update.
    /// </summary>
    public required int CustomerId { get; set; }

    /// <summary>
    /// Customer's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Customer's last name.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Customer's email address (must be unique if changed).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Customer's phone number.
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Customer's street address.
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Customer's city.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Customer's state.
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    /// Customer's zip code.
    /// </summary>
    public required string ZipCode { get; set; }

    /// <summary>
    /// Customer's date of birth.
    /// </summary>
    public required DateTime DateOfBirth { get; set; }
}

/// <summary>
/// Command to delete (soft-delete) a customer by their unique identifier.
/// </summary>
public class DeleteCustomerCommand : IRequest<Result>
{
    /// <summary>
    /// The unique identifier of the customer to delete.
    /// </summary>
    public required int CustomerId { get; set; }
}

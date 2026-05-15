using Insurance.Application.Common.Models;
using Insurance.Application.Features.Customers.DTOs;
using MediatR;

namespace Insurance.Application.Features.Customers.Queries;

/// <summary>
/// Query to retrieve all customers with pagination support.
/// </summary>
public class GetAllCustomersQuery : IRequest<PaginatedResult<CustomerListDto>>
{
    /// <summary>
    /// Page number (1-based) for pagination.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Query to retrieve a customer by their unique identifier.
/// </summary>
public class GetCustomerByIdQuery : IRequest<Result<CustomerResponseDto?>>
{
    /// <summary>
    /// The unique customer identifier to retrieve.
    /// </summary>
    public required int CustomerId { get; set; }
}

/// <summary>
/// Query to search for customers by name (first or last name).
/// </summary>
public class SearchCustomersByNameQuery : IRequest<Result<IEnumerable<CustomerListDto>>>
{
    /// <summary>
    /// Search term to match against first or last name (case-insensitive).
    /// </summary>
    public required string SearchTerm { get; set; }
}

/// <summary>
/// Query to retrieve customers by city.
/// </summary>
public class GetCustomersByCityQuery : IRequest<Result<IEnumerable<CustomerListDto>>>
{
    /// <summary>
    /// City name to filter customers.
    /// </summary>
    public required string City { get; set; }
}

/// <summary>
/// Query to retrieve customers by state.
/// </summary>
public class GetCustomersByStateQuery : IRequest<Result<IEnumerable<CustomerListDto>>>
{
    /// <summary>
    /// State abbreviation (e.g., "CA", "NY") to filter customers.
    /// </summary>
    public required string State { get; set; }
}

/// <summary>
/// Query to retrieve customers by zip code.
/// </summary>
public class GetCustomersByZipCodeQuery : IRequest<Result<IEnumerable<CustomerListDto>>>
{
    /// <summary>
    /// Zip code to filter customers.
    /// </summary>
    public required string ZipCode { get; set; }
}

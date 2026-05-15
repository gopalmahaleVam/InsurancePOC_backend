using Insurance.Application.Common.Models;
using Insurance.Application.Features.Customers.Commands;
using Insurance.Application.Features.Customers.DTOs;
using Insurance.Application.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers;

/// <summary>
/// API controller for managing customers.
/// Provides endpoints for CRUD operations and customer searches.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Gets a paginated list of all customers.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based). Default is 1.</param>
    /// <param name="pageSize">Number of items per page. Default is 10, maximum is 100.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// A paginated list of customers with pagination metadata.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/customers?pageNumber=1&pageSize=10
    ///
    /// Sample response:
    ///
    ///     {
    ///       "data": [
    ///         {
    ///           "id": 1,
    ///           "firstName": "John",
    ///           "lastName": "Doe",
    ///           "fullName": "John Doe",
    ///           "email": "john@example.com",
    ///           "phoneNumber": "555-0001",
    ///           "city": "New York",
    ///           "state": "NY",
    ///           "dateOfBirth": "1990-01-15",
    ///           "age": 34
    ///         }
    ///       ],
    ///       "pageNumber": 1,
    ///       "pageSize": 10,
    ///       "totalCount": 50
    ///     }
    /// </remarks>
    /// <response code="200">Successfully retrieved customers.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<CustomerListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResult<CustomerListDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCustomersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific customer by ID.
    /// </summary>
    /// <param name="id">The unique customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// The customer with the specified ID if found.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/customers/1
    ///
    /// Sample response:
    ///
    ///     {
    ///       "success": true,
    ///       "message": null,
    ///       "data": {
    ///         "id": 1,
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "fullName": "John Doe",
    ///         "email": "john@example.com",
    ///         "phoneNumber": "555-0001",
    ///         "address": "123 Main St",
    ///         "city": "New York",
    ///         "state": "NY",
    ///         "zipCode": "10001",
    ///         "fullAddress": "123 Main St, New York, NY 10001",
    ///         "dateOfBirth": "1990-01-15",
    ///         "age": 34,
    ///         "createdAt": "2024-01-01T10:00:00Z",
    ///         "updatedAt": "2024-01-01T10:00:00Z"
    ///       }
    ///     }
    /// </remarks>
    /// <response code="200">Customer found and returned.</response>
    /// <response code="404">Customer not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<CustomerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<CustomerResponseDto>>> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerByIdQuery { CustomerId = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, message = result.ErrorMessage });

        return Ok(result);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="createCustomerDto">Customer information to create.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// The created customer with its new ID.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/customers
    ///     {
    ///       "firstName": "Jane",
    ///       "lastName": "Smith",
    ///       "email": "jane@example.com",
    ///       "phoneNumber": "555-0002",
    ///       "address": "456 Oak Ave",
    ///       "city": "Los Angeles",
    ///       "state": "CA",
    ///       "zipCode": "90001",
    ///       "dateOfBirth": "1985-06-20"
    ///     }
    /// </remarks>
    /// <response code="201">Customer created successfully.</response>
    /// <response code="400">Invalid customer data or email already exists.</response>
    /// <response code="422">Validation failed.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<CustomerResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<Result<CustomerResponseDto>>> Create(
        [FromBody] CreateCustomerDto createCustomerDto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCustomerCommand
        {
            FirstName = createCustomerDto.FirstName,
            LastName = createCustomerDto.LastName,
            Email = createCustomerDto.Email,
            PhoneNumber = createCustomerDto.PhoneNumber,
            Address = createCustomerDto.Address,
            City = createCustomerDto.City,
            State = createCustomerDto.State,
            ZipCode = createCustomerDto.ZipCode,
            DateOfBirth = createCustomerDto.DateOfBirth
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">The unique customer identifier.</param>
    /// <param name="updateCustomerDto">Updated customer information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// The updated customer.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/customers/1
    ///     {
    ///       "firstName": "Jane",
    ///       "lastName": "Smith",
    ///       "email": "jane.smith@example.com",
    ///       "phoneNumber": "555-1234",
    ///       "address": "789 Pine St",
    ///       "city": "Los Angeles",
    ///       "state": "CA",
    ///       "zipCode": "90002",
    ///       "dateOfBirth": "1985-06-20"
    ///     }
    /// </remarks>
    /// <response code="200">Customer updated successfully.</response>
    /// <response code="404">Customer not found.</response>
    /// <response code="400">Invalid data or email already exists.</response>
    /// <response code="422">Validation failed.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<CustomerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<Result<CustomerResponseDto>>> Update(
        int id,
        [FromBody] UpdateCustomerDto updateCustomerDto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCustomerCommand
        {
            CustomerId = id,
            FirstName = updateCustomerDto.FirstName,
            LastName = updateCustomerDto.LastName,
            Email = updateCustomerDto.Email,
            PhoneNumber = updateCustomerDto.PhoneNumber,
            Address = updateCustomerDto.Address,
            City = updateCustomerDto.City,
            State = updateCustomerDto.State,
            ZipCode = updateCustomerDto.ZipCode,
            DateOfBirth = updateCustomerDto.DateOfBirth
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Deletes a customer (soft delete).
    /// </summary>
    /// <param name="id">The unique customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// Success message if deletion was successful.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/customers/1
    ///
    /// Note: This performs a soft delete. The customer record is marked as deleted in the database
    /// but the data is retained for audit and recovery purposes.
    /// </remarks>
    /// <response code="200">Customer deleted successfully.</response>
    /// <response code="404">Customer not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCustomerCommand { CustomerId = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true });
    }

    /// <summary>
    /// Searches for customers by name (first or last name).
    /// </summary>
    /// <param name="searchTerm">Search term to find in first or last name.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// List of customers matching the search criteria.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/customers/search/name?searchTerm=John
    ///
    /// Returns all customers with first or last name containing "John".
    /// </remarks>
    /// <response code="200">Search completed successfully.</response>
    /// <response code="400">Invalid search term.</response>
    [HttpGet("search/name")]
    [ProducesResponseType(typeof(Result<IEnumerable<CustomerListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IEnumerable<CustomerListDto>>>> SearchByName(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchCustomersByNameQuery { SearchTerm = searchTerm };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Gets all customers in a specific city.
    /// </summary>
    /// <param name="city">City name to filter by.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// List of customers in the specified city.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/customers/search/city?city=New%20York
    /// </remarks>
    /// <response code="200">Query completed successfully.</response>
    /// <response code="400">Invalid city name.</response>
    [HttpGet("search/city")]
    [ProducesResponseType(typeof(Result<IEnumerable<CustomerListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IEnumerable<CustomerListDto>>>> GetByCity(
        [FromQuery] string city,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomersByCityQuery { City = city };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Gets all customers in a specific state.
    /// </summary>
    /// <param name="state">State abbreviation (e.g., "CA", "NY") to filter by.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// List of customers in the specified state.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/customers/search/state?state=CA
    /// </remarks>
    /// <response code="200">Query completed successfully.</response>
    /// <response code="400">Invalid state.</response>
    [HttpGet("search/state")]
    [ProducesResponseType(typeof(Result<IEnumerable<CustomerListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IEnumerable<CustomerListDto>>>> GetByState(
        [FromQuery] string state,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomersByStateQuery { State = state };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Gets all customers with a specific zip code.
    /// </summary>
    /// <param name="zipCode">Zip code to filter by.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// List of customers with the specified zip code.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/customers/search/zipcode?zipCode=10001
    /// </remarks>
    /// <response code="200">Query completed successfully.</response>
    /// <response code="400">Invalid zip code.</response>
    [HttpGet("search/zipcode")]
    [ProducesResponseType(typeof(Result<IEnumerable<CustomerListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IEnumerable<CustomerListDto>>>> GetByZipCode(
        [FromQuery] string zipCode,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomersByZipCodeQuery { ZipCode = zipCode };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, data = result.Data });
    }
}

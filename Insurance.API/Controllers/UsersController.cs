
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Users.Commands;
using Insurance.Application.Features.Users.DTOs;
using Insurance.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers;

/// <summary>
/// Users management REST API endpoints.
/// Provides endpoints for CRUD operations on users with role-based access control.
/// All operations dispatch through MediatR for CQRS pattern compliance.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new user account with the provided credentials and profile information.
    /// Username and email must be unique in the system.
    /// </summary>
    /// <param name="createUserDto">User creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user with 201 status</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Validation error or duplicate username/email</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto createUserDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new user with username: {Username}", createUserDto.Username);

        var command = new CreateUserCommand
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            Password = createUserDto.Password,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            PhoneNumber = createUserDto.PhoneNumber,
            Role = createUserDto.Role
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to create user: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        _logger.LogInformation("User created successfully with ID: {UserId}", result.Data?.Id);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Data?.Id }, result.Data);
    }

    /// <summary>
    /// Retrieves a specific user by ID.
    /// Returns user details excluding password hash.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details if found</returns>
    /// <response code="200">User found and returned</response>
    /// <response code="404">User not found</response>
    [HttpGet("get/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, message = result.ErrorMessage });

        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Retrieves a paginated list of all users.
    /// Supports filtering by role, active status, and search term.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Optional search term (filters username, email, name)</param>
    /// <param name="roleFilter">Optional role filter</param>
    /// <param name="isActiveFilter">Optional active status filter</param>
    /// <param name="sortBy">Sort field (default: -createdAt)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    /// <response code="200">Users list returned</response>
    /// <response code="400">Invalid pagination parameters</response>
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Insurance.Domain.Enums.UserRole? roleFilter = null,
        [FromQuery] bool? isActiveFilter = null,
        [FromQuery] string? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            RoleFilter = roleFilter,
            IsActiveFilter = isActiveFilter,
            SortBy = sortBy ?? "-createdAt"
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new
        {
            success = true,
            data = result.Data?.Items,
            pagination = new
            {
                pageNumber = result.Data?.PageNumber,
                pageSize = result.Data?.PageSize,
                totalItems = result.Data?.TotalItems,
                totalPages = result.Data?.TotalPages,
                hasNextPage = result.Data?.HasNextPage,
                hasPreviousPage = result.Data?.HasPreviousPage
            }
        });
    }

    /// <summary>
    /// Updates user profile information (first name, last name, phone, email, role, active status).
    /// Note: Password changes must use a separate endpoint for security.
    /// </summary>
    /// <param name="id">User ID to update</param>
    /// <param name="updateUserDto">Updated user data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user details</returns>
    /// <response code="200">User updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">User not found</response>
    [HttpPut("update/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        int id,
        [FromBody] UpdateUserDto updateUserDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);

        var command = new UpdateUserCommand
        {
            Id = id,
            FirstName = updateUserDto.FirstName,
            LastName = updateUserDto.LastName,
            PhoneNumber = updateUserDto.PhoneNumber,
            Email = updateUserDto.Email,
            Role = updateUserDto.Role,
            IsActive = updateUserDto.IsActive
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update user {UserId}: {ErrorMessage}", id, result.ErrorMessage);
            return NotFound(new { success = false, message = result.ErrorMessage });
        }

        _logger.LogInformation("User {UserId} updated successfully", id);
        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Soft-deletes a user from the system.
    /// The user record remains in the database but is marked as deleted.
    /// </summary>
    /// <param name="id">User ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">User deleted successfully</response>
    /// <response code="404">User not found</response>
    [HttpDelete("delete/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);

        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete user {UserId}: {ErrorMessage}", id, result.ErrorMessage);
            return NotFound(new { success = false, message = result.ErrorMessage });
        }

        _logger.LogInformation("User {UserId} deleted successfully", id);
        return NoContent();
    }

    /// <summary>
    /// Searches users by first or last name (case-insensitive).
    /// </summary>
    /// <param name="searchTerm">Name search term (minimum 2 characters)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching users</returns>
    /// <response code="200">Search results returned</response>
    /// <response code="400">Invalid search term</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchByName(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            return BadRequest(new { success = false, message = "Search term must be at least 2 characters" });

        var query = new SearchUsersByNameQuery { SearchTerm = searchTerm };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Gets all users with a specific role.
    /// </summary>
    /// <param name="role">User role to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users with specified role</returns>
    /// <response code="200">Users list returned</response>
    [HttpGet("by-role/{role}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByRole(
        Insurance.Domain.Enums.UserRole role,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersByRoleQuery { Role = role };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new { success = true, data = result.Data });
    }

    /// <summary>
    /// Checks if a username is available for registration.
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if available, false if taken</returns>
    /// <response code="200">Availability returned</response>
    [HttpGet("check-username/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckUsernameAvailability(
        string username,
        CancellationToken cancellationToken)
    {
        var query = new CheckUsernameAvailabilityQuery { Username = username };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new { success = true, available = result.Data });
    }

    /// <summary>
    /// Checks if an email is available for registration.
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if available, false if taken</returns>
    /// <response code="200">Availability returned</response>
    [HttpGet("check-email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckEmailAvailability(
        string email,
        CancellationToken cancellationToken)
    {
        var query = new CheckEmailAvailabilityQuery { Email = email };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new { success = true, available = result.Data });
    }
}

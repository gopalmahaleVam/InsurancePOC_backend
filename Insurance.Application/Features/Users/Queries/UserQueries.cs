using Insurance.Application.Common.Models;
using Insurance.Application.Features.Users.DTOs;
using MediatR;

namespace Insurance.Application.Features.Users.Queries;

/// <summary>
/// Query to retrieve a user by ID.
/// Returns null if user not found or is deleted.
/// </summary>
public class GetUserByIdQuery : IRequest<Result<UserResponseDto>>
{
    /// <summary>
    /// User ID to retrieve.
    /// </summary>
    public int Id { get; set; }

    public GetUserByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Query to retrieve all users with pagination.
/// Excludes deleted users by default.
/// Supports optional searching and sorting.
/// </summary>
public class GetAllUsersQuery : IRequest<Result<PaginatedResult<UserListDto>>>
{
    /// <summary>
    /// Current page number (1-based). Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10, maximum 100.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Optional search term to filter users by name or email.
    /// Searches first name, last name, email, and username.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Optional filter for user role.
    /// If specified, returns only users with this role.
    /// </summary>
    public Insurance.Domain.Enums.UserRole? RoleFilter { get; set; }

    /// <summary>
    /// Optional filter for active status.
    /// If specified, filters by IsActive value.
    /// </summary>
    public bool? IsActiveFilter { get; set; }

    /// <summary>
    /// Sort field (e.g., "username", "email", "createdAt").
    /// Prefix with '-' for descending order (e.g., "-createdAt").
    /// </summary>
    public string SortBy { get; set; } = "-createdAt";
}

/// <summary>
/// Query to search users by name (first name or last name).
/// Returns matching users without pagination.
/// </summary>
public class SearchUsersByNameQuery : IRequest<Result<IEnumerable<UserResponseDto>>>
{
    /// <summary>
    /// Search term to find in first or last name.
    /// </summary>
    public required string SearchTerm { get; set; }
}

/// <summary>
/// Query to retrieve all users with a specific role.
/// </summary>
public class GetUsersByRoleQuery : IRequest<Result<IEnumerable<UserResponseDto>>>
{
    /// <summary>
    /// User role to filter by.
    /// </summary>
    public required Insurance.Domain.Enums.UserRole Role { get; set; }
}

/// <summary>
/// Query to check if a username is already taken.
/// Used for form validation before submission.
/// </summary>
public class CheckUsernameAvailabilityQuery : IRequest<Result<bool>>
{
    /// <summary>
    /// Username to check.
    /// </summary>
    public required string Username { get; set; }
}

/// <summary>
/// Query to check if an email is already taken.
/// Used for form validation before submission.
/// </summary>
public class CheckEmailAvailabilityQuery : IRequest<Result<bool>>
{
    /// <summary>
    /// Email to check.
    /// </summary>
    public required string Email { get; set; }
}

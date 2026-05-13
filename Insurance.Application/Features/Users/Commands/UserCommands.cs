using Insurance.Application.Common.Models;
using Insurance.Application.Features.Users.DTOs;
using MediatR;

namespace Insurance.Application.Features.Users.Commands;

/// <summary>
/// Command to create a new user in the system.
/// Creates user account with provided credentials and profile information.
/// Password is bcrypt-hashed server-side before storage.
/// Triggers validation for uniqueness of username and email.
/// </summary>
public class CreateUserCommand : IRequest<Result<UserResponseDto>>
{
    /// <summary>
    /// Unique username for authentication.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Email address (must be unique).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Plain-text password (will be hashed before storage).
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Optional phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's role in the system.
    /// </summary>
    public required Insurance.Domain.Enums.UserRole Role { get; set; }
}

/// <summary>
/// Command to update an existing user's profile information.
/// Does NOT handle password changes (separate endpoint required).
/// Can update first name, last name, phone, email, role, and active status.
/// </summary>
public class UpdateUserCommand : IRequest<Result<UserResponseDto>>
{
    /// <summary>
    /// User ID to update (from route parameter).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Updated first name (optional - only updated if provided).
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Updated last name (optional).
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Updated phone number (optional).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Updated email address (must be unique if changed).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Updated user role (optional).
    /// </summary>
    public Insurance.Domain.Enums.UserRole? Role { get; set; }

    /// <summary>
    /// Whether to activate or deactivate the account (optional).
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Command to soft-delete a user from the system.
/// Marks the user as deleted rather than removing from database.
/// Preserves data for audit and recovery purposes.
/// </summary>
public class DeleteUserCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// ID of the user to delete.
    /// </summary>
    public int Id { get; set; }

    public DeleteUserCommand(int id)
    {
        Id = id;
    }
}

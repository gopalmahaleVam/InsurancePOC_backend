using Insurance.Domain.Enums;

namespace Insurance.Application.Features.Users.DTOs;

/// <summary>
/// DTO for creating a new user via API.
/// Contains only the fields required for user creation; excludes audit fields and system-generated properties.
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Unique username for authentication (3-50 characters).
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Valid email address (must be unique in system).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Plain-text password (will be hashed server-side).
    /// Minimum 8 characters, must include uppercase, lowercase, digit.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// User's first name (1-100 characters).
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name (1-100 characters).
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Optional phone number for contact.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's role in the system.
    /// Determines authorization level and feature access.
    /// </summary>
    public required UserRole Role { get; set; }
}

/// <summary>
/// DTO for updating an existing user via API.
/// Allows partial updates while maintaining security by excluding password changes.
/// Password must be changed through dedicated password change endpoint.
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// User's ID (from route parameter).
    /// Included for command routing and validation.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Updated first name (1-100 characters).
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Updated last name (1-100 characters).
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Updated phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Updated email address.
    /// Must be unique in system if changed.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Updated user role.
    /// </summary>
    public UserRole? Role { get; set; }

    /// <summary>
    /// Whether to activate or deactivate the user account.
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO for user responses in API endpoints.
/// Safe to return in API responses; excludes password hash and sensitive audit data.
/// </summary>
public class UserResponseDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Full name computed from FirstName and LastName.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// User's role in the system.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Whether the user account is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Timestamp when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last update.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last successful login.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Lightweight DTO for user list items.
/// Used in paginated list endpoints to minimize payload size.
/// </summary>
public class UserListDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Timestamp of creation.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

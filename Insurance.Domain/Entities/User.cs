using Insurance.Domain.Enums;

namespace Insurance.Domain.Entities;

/// <summary>
/// Represents a system user with authentication, authorization, and profile information.
/// Includes soft delete and audit tracking for security and compliance.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Unique username for authentication.
    /// Must be unique and non-null.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// User's email address for communication and account recovery.
    /// Must be unique and in valid email format.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Bcrypt hashed password. Never stored in plain text.
    /// Updated when user changes password.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// User's phone number for contact purposes.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's role in the system (Admin, Agent, Customer, etc.).
    /// Determines authorization level and feature access.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Indicates if the user account is currently active.
    /// Active users can authenticate; inactive users cannot.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp of the last successful login (UTC).
    /// Used for security monitoring and compliance reporting.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Creates a new user with the specified credentials and profile.
    /// </summary>
    /// <param name="username">Unique username for authentication</param>
    /// <param name="email">Unique email address</param>
    /// <param name="passwordHash">Bcrypt hashed password</param>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="role">User role in system</param>
    /// <param name="phoneNumber">Optional phone number</param>
    public User(string username, string email, string passwordHash, string firstName, string lastName, UserRole role, string? phoneNumber = null)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        PhoneNumber = phoneNumber;
    }
}

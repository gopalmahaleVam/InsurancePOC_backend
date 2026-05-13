using Insurance.Domain.Entities;

namespace Insurance.Domain.Interfaces;

/// <summary>
/// User-specific repository interface.
/// Extends generic repository with user-specific queries.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves a user by username (case-insensitive), excluding deleted users.
    /// Used during authentication and username availability checks.
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user if found; null otherwise</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by email address (case-insensitive), excluding deleted users.
    /// Used for authentication, email verification, and email availability checks.
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user if found; null otherwise</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for users whose first or last name contains the search term (case-insensitive).
    /// Excludes deleted users.
    /// </summary>
    /// <param name="searchTerm">The term to search in first and last names</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of matching users</returns>
    Task<IEnumerable<User>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with a specific role, excluding deleted users.
    /// Used for role-based user listings and administration.
    /// </summary>
    /// <param name="role">The role to filter by</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users with the specified role</returns>
    Task<IEnumerable<User>> GetByRoleAsync(Insurance.Domain.Enums.UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username already exists in the system (case-insensitive).
    /// Excludes deleted users from the check, allowing username reuse after deletion.
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if username exists; false otherwise</returns>
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email already exists in the system (case-insensitive).
    /// Excludes deleted users from the check, allowing email reuse after deletion.
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if email exists; false otherwise</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
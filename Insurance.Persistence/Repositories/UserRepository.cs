
using Insurance.Domain.Entities;
using Insurance.Domain.Enums;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories;

/// <summary>
/// User repository implementation providing user-specific data access operations.
/// Extends the generic repository with queries specific to user management and authentication.
/// All queries exclude soft-deleted users by default.
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(InsuranceDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves a user by username (case-insensitive), excluding deleted users.
    /// Used during authentication and username availability checks.
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && !u.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by email address (case-insensitive), excluding deleted users.
    /// Used for authentication, email verification, and email availability checks.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Searches for users whose first or last name contains the search term (case-insensitive).
    /// Excludes deleted users.
    /// </summary>
    public async Task<IEnumerable<User>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(u => (u.FirstName.ToLower().Contains(lowerSearchTerm) || 
                        u.LastName.ToLower().Contains(lowerSearchTerm)) && 
                        !u.IsDeleted)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all users with a specific role, excluding deleted users.
    /// Used for role-based user listings and administration.
    /// </summary>
    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.Role == role && !u.IsDeleted)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a username already exists in the system (case-insensitive).
    /// Excludes deleted users from the check, allowing username reuse after deletion.
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Username.ToLower() == username.ToLower() && !u.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Checks if an email already exists in the system (case-insensitive).
    /// Excludes deleted users from the check, allowing email reuse after deletion.
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted, cancellationToken);
    }
}

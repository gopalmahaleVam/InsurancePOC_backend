using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories;

/// <summary>
/// Customer repository implementation providing customer-specific data access operations.
/// Extends the generic repository with queries specific to customer management and lifecycle.
/// All queries exclude soft-deleted customers by default.
/// </summary>
public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(InsuranceDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves a customer by email address (case-insensitive), excluding deleted customers.
    /// Used for customer lookup and email availability checks.
    /// </summary>
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower() && !c.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Searches for customers whose first or last name contains the search term (case-insensitive).
    /// Excludes deleted customers.
    /// </summary>
    public async Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(c => (c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                        c.LastName.ToLower().Contains(lowerSearchTerm)) &&
                        !c.IsDeleted)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves customers by city, excluding deleted customers.
    /// Used for geographic customer distribution and reporting.
    /// </summary>
    public async Task<IEnumerable<Customer>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.City.ToLower() == city.ToLower() && !c.IsDeleted)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves customers by state, excluding deleted customers.
    /// Used for regional customer management and reporting.
    /// </summary>
    public async Task<IEnumerable<Customer>> GetByStateAsync(string state, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.State.ToUpper() == state.ToUpper() && !c.IsDeleted)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a customer with the given email already exists in the system (case-insensitive).
    /// Excludes deleted customers from the check, allowing email reuse after deletion.
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(c => c.Email.ToLower() == email.ToLower() && !c.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Retrieves customers by zip code, excluding deleted customers.
    /// Used for area-based customer queries and demographics.
    /// </summary>
    public async Task<IEnumerable<Customer>> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.ZipCode == zipCode && !c.IsDeleted)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);
    }
}

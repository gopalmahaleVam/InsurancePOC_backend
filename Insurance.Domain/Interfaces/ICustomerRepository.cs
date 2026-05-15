using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;

namespace Insurance.Domain.Interfaces;

/// <summary>
/// Customer repository interface providing customer-specific data access operations.
/// Extends the generic repository with queries specific to customer management and lifecycle.
/// All queries exclude soft-deleted customers by default.
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Retrieves a customer by email address (case-insensitive), excluding deleted customers.
    /// Used for customer lookup and email availability checks.
    /// </summary>
    /// <param name="email">The customer's email address</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The customer if found; null otherwise</returns>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for customers whose first or last name contains the search term (case-insensitive).
    /// Excludes deleted customers.
    /// </summary>
    /// <param name="searchTerm">The search term to find in first or last name</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of customers matching the search criteria</returns>
    Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves customers by city, excluding deleted customers.
    /// Used for geographic customer distribution and reporting.
    /// </summary>
    /// <param name="city">The city to filter by</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of customers in the specified city</returns>
    Task<IEnumerable<Customer>> GetByCityAsync(string city, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves customers by state, excluding deleted customers.
    /// Used for regional customer management and reporting.
    /// </summary>
    /// <param name="state">The state abbreviation (e.g., "CA", "NY")</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of customers in the specified state</returns>
    Task<IEnumerable<Customer>> GetByStateAsync(string state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a customer with the given email already exists in the system (case-insensitive).
    /// Excludes deleted customers from the check, allowing email reuse after deletion.
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if email exists; false otherwise</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves customers by zip code, excluding deleted customers.
    /// Used for area-based customer queries and demographics.
    /// </summary>
    /// <param name="zipCode">The zip code to filter by</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of customers in the specified zip code</returns>
    Task<IEnumerable<Customer>> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default);
}

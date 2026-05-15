using Insurance.Domain.Entities;

namespace Insurance.Domain.Interfaces;

/// <summary>
/// Repository interface for policies.
/// Extends generic repository with policy-specific queries.
/// </summary>
public interface IPolicyRepository : IRepository<Policy>
{
    /// <summary>
    /// Retrieves policies for a given customer.
    /// </summary>
    Task<IEnumerable<Policy>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves policies for a given product.
    /// </summary>
    Task<IEnumerable<Policy>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches policies by policy number.
    /// </summary>
    Task<Policy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default);
}

using Insurance.Domain.Entities;

namespace Insurance.Domain.Interfaces;

/// <summary>
/// Repository interface for claims.
/// Extends generic repository with claim-specific queries.
/// </summary>
public interface IClaimRepository : IRepository<Claim>
{
    /// <summary>
    /// Retrieves claims for a given policy.
    /// </summary>
    Task<IEnumerable<Claim>> GetByPolicyIdAsync(int policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves claims for a given customer.
    /// </summary>
    Task<IEnumerable<Claim>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches a claim by claim number.
    /// </summary>
    Task<Claim?> GetByClaimNumberAsync(string claimNumber, CancellationToken cancellationToken = default);
}

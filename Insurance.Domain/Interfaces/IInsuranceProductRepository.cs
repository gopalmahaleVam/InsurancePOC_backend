using Insurance.Domain.Entities;

namespace Insurance.Domain.Interfaces;

/// <summary>
/// Repository interface for insurance products.
/// Extends generic repository with product-specific queries.
/// </summary>
public interface IInsuranceProductRepository : IRepository<InsuranceProduct>
{
    /// <summary>
    /// Retrieves active products by type (e.g., Auto, Home).
    /// </summary>
    Task<IEnumerable<InsuranceProduct>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches products by name or description.
    /// </summary>
    Task<IEnumerable<InsuranceProduct>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

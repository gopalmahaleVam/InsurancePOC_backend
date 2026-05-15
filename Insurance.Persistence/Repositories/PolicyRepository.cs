using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories;

public class PolicyRepository : GenericRepository<Policy>, IPolicyRepository
{
    public PolicyRepository(InsuranceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Policy>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.CustomerId == customerId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Policy>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.InsuranceProductId == productId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Policy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PolicyNumber.ToLower() == policyNumber.ToLower() && !p.IsDeleted, cancellationToken);
    }
}

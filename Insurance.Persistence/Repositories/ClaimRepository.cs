using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories;

public class ClaimRepository : GenericRepository<Claim>, IClaimRepository
{
    public ClaimRepository(InsuranceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Claim>> GetByPolicyIdAsync(int policyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.PolicyId == policyId && !c.IsDeleted)
            .OrderByDescending(c => c.ClaimDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Claim>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId && !c.IsDeleted)
            .OrderByDescending(c => c.ClaimDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Claim?> GetByClaimNumberAsync(string claimNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClaimNumber.ToLower() == claimNumber.ToLower() && !c.IsDeleted, cancellationToken);
    }
}

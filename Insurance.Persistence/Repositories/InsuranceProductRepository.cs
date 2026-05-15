using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories;

public class InsuranceProductRepository : GenericRepository<InsuranceProduct>, IInsuranceProductRepository
{
    public InsuranceProductRepository(InsuranceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InsuranceProduct>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        var lowerType = (type ?? string.Empty).ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Type.ToLower() == lowerType && !p.IsDeleted)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InsuranceProduct>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(cancellationToken);

        var lower = searchTerm.ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(p => (p.Name.ToLower().Contains(lower) || p.Description.ToLower().Contains(lower)) && !p.IsDeleted)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}

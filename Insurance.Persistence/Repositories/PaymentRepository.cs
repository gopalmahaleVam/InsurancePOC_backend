using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(InsuranceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByPolicyIdAsync(int policyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.PolicyId == policyId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.CustomerId == customerId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.TransactionId.ToLower() == transactionId.ToLower() && !p.IsDeleted, cancellationToken);
    }
}

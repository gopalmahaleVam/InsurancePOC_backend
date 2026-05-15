using Insurance.Domain.Entities;

namespace Insurance.Domain.Interfaces;

/// <summary>
/// Repository interface for payments.
/// Extends generic repository with payment-specific queries.
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>
    /// Retrieves payments for a specific policy.
    /// </summary>
    Task<IEnumerable<Payment>> GetByPolicyIdAsync(int policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves payments for a specific customer.
    /// </summary>
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a payment by transaction id.
    /// </summary>
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
}

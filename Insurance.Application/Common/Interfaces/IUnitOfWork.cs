using Insurance.Domain.Interfaces;

namespace Insurance.Application.Common.Interfaces;

/// <summary>
/// Unit of Work pattern interface for transaction management.
/// Coordinates multiple repositories and ensures all changes are committed atomically.
/// Prevents repositories from individually calling SaveChanges, centralizing transaction control.
/// This interface must be implemented in the Persistence layer.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// User repository for user-related data access.
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Customer repository for customer-related data access.
    /// </summary>
    ICustomerRepository Customers { get; }

    /// <summary>
    /// Insurance product repository for product-related data access.
    /// </summary>
    Insurance.Domain.Interfaces.IInsuranceProductRepository Products { get; }

    /// <summary>
    /// Commits all pending changes to the database in a single transaction.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Number of entities affected</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new transaction for this unit of work.
    /// Useful for explicit transaction control or for rollback scenarios.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A transaction object that can be committed or rolled back</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// Must be called after BeginTransactionAsync to finalize changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction, undoing all pending changes.
    /// Called automatically if an exception occurs.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

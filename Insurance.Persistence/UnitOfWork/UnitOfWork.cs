using Insurance.Application.Common.Interfaces;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Insurance.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Insurance.Persistence.UnitOfWork;

/// <summary>
/// Unit of Work pattern implementation for coordinating repository transactions.
/// Manages transaction boundaries and ensures atomic operations across multiple repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly InsuranceDbContext _context;
    private IDbContextTransaction? _transaction;
    private IUserRepository? _userRepository;
    private ICustomerRepository? _customerRepository;
    private Insurance.Domain.Interfaces.IInsuranceProductRepository? _productRepository;
    private Insurance.Domain.Interfaces.IPolicyRepository? _policyRepository;
    private Insurance.Domain.Interfaces.IClaimRepository? _claimRepository;
    private Insurance.Domain.Interfaces.IPaymentRepository? _paymentRepository;

    public UnitOfWork(InsuranceDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets the user repository, lazily instantiating it on first access.
    /// This prevents creating unused repositories for operations that don't need them.
    /// </summary>
    public IUserRepository Users
    {
        get
        {
            _userRepository ??= new UserRepository(_context);
            return _userRepository;
        }
    }

    /// <summary>
    /// Gets the customer repository, lazily instantiating it on first access.
    /// This prevents creating unused repositories for operations that don't need them.
    /// </summary>
    public ICustomerRepository Customers
    {
        get
        {
            _customerRepository ??= new CustomerRepository(_context);
            return _customerRepository;
        }
    }

    /// <summary>
    /// Gets the insurance product repository, lazily instantiating it on first access.
    /// </summary>
    public Insurance.Domain.Interfaces.IInsuranceProductRepository Products
    {
        get
        {
            _productRepository ??= new InsuranceProductRepository(_context);
            return _productRepository;
        }
    }

    /// <summary>
    /// Gets the policy repository, lazily instantiating it on first access.
    /// </summary>
    public Insurance.Domain.Interfaces.IPolicyRepository Policies
    {
        get
        {
            _policyRepository ??= new PolicyRepository(_context);
            return _policyRepository;
        }
    }

    /// <summary>
    /// Gets the claim repository, lazily instantiating it on first access.
    /// </summary>
    public Insurance.Domain.Interfaces.IClaimRepository Claims
    {
        get
        {
            _claimRepository ??= new ClaimRepository(_context);
            return _claimRepository;
        }
    }

    /// <summary>
    /// Gets the payment repository, lazily instantiating it on first access.
    /// </summary>
    public Insurance.Domain.Interfaces.IPaymentRepository Payments
    {
        get
        {
            _paymentRepository ??= new PaymentRepository(_context);
            return _paymentRepository;
        }
    }

    /// <summary>
    /// Saves all pending changes to the database in a single transaction.
    /// This method ensures data consistency by treating all changes atomically.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Number of database entities affected by this save operation</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins a new database transaction for explicit transaction control.
    /// Allows callers to define transaction boundaries explicitly.
    /// All repository operations will use this transaction until committed or rolled back.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction, persisting all changes to the database.
    /// Must be called after BeginTransactionAsync to finalize pending changes.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction, discarding all pending changes.
    /// Useful for error handling to ensure database consistency when operations fail.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the unit of work and releases database resources.
    /// Ensures transactions are properly cleaned up.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}

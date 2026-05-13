
using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories;

/// <summary>
/// Generic repository implementation providing common CRUD operations for all entities.
/// Handles soft delete logic by filtering out IsDeleted = true from queries.
/// Designed for use with Unit of Work pattern to manage transaction scope.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository</typeparam>
public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly InsuranceDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(InsuranceDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Retrieves an entity by ID, excluding soft-deleted entries.
    /// </summary>
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Retrieves all active (non-deleted) entities.
    /// </summary>
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated collection of active entities ordered by creation date (descending).
    /// </summary>
    public async Task<(IEnumerable<T> items, int totalCount)> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet
            .Where(e => !e.IsDeleted)
            .CountAsync(cancellationToken);

        var items = await _dbSet
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Adds a new entity to the context (does not persist until SaveChangesAsync is called).
    /// </summary>
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Adds multiple entities to the context in a single operation.
    /// </summary>
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// Updates an entity in the context (does not persist until SaveChangesAsync is called).
    /// Automatically updates the UpdatedAt timestamp.
    /// </summary>
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Soft-deletes an entity by marking it as deleted rather than removing it.
    /// This preserves the entity for audit purposes while making it invisible to normal queries.
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
        if (entity is null)
            return false;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Checks if an active (non-deleted) entity with the specified ID exists.
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }
}
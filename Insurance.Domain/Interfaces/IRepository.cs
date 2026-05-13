using Insurance.Domain.Entities;

namespace Insurance.Domain.Interfaces;

/// <summary>
/// Generic repository interface for data access operations.
/// Provides common CRUD operations for all entities with soft delete support.
/// Implementation must handle IsDeleted flag for all queries.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Retrieves an entity by its ID, excluding soft-deleted entities.
    /// </summary>
    /// <param name="id">The entity's primary key</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The entity if found and not deleted; null otherwise</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active (non-deleted) entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all active entities</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated collection of active entities.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Tuple containing the page items and total count</returns>
    Task<(IEnumerable<T> items, int totalCount)> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the repository (does not save immediately).
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities to the repository (does not save immediately).
    /// </summary>
    /// <param name="entities">Collection of entities to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity (does not save immediately).
    /// </summary>
    /// <param name="entity">The entity with updated values</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes an entity by ID (marks IsDeleted = true, does not remove from database).
    /// </summary>
    /// <param name="id">The ID of the entity to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if entity existed and was deleted; false if not found</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an entity with the specified ID exists and is not deleted.
    /// </summary>
    /// <param name="id">The entity's primary key</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if entity exists and is active; false otherwise</returns>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

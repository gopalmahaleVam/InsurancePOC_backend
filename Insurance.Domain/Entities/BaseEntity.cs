namespace Insurance.Domain.Entities;

/// <summary>
/// Base entity class providing common audit and soft-delete functionality across all domain entities.
/// Ensures consistent timestamp and logical deletion tracking across the domain model.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Timestamp when the entity was created (UTC).
    /// Set automatically during entity creation.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the entity was last updated (UTC).
    /// Updated automatically on entity modifications.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if the entity is logically deleted.
    /// Supports soft delete pattern for audit and recovery purposes.
    /// </summary>
    public bool IsDeleted { get; set; }
}

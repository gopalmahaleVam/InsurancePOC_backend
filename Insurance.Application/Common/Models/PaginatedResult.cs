namespace Insurance.Application.Common.Models;

/// <summary>
/// Generic paginated response wrapper for list queries.
/// Ensures consistent pagination across all list endpoints with metadata about total items and pages.
/// </summary>
/// <typeparam name="T">The type of items in the paginated list</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// The collection of items for this page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; set; }

    /// <summary>
    /// Current page number (1-based indexing).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages (before pagination).
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages based on PageSize and TotalItems.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates if there are more pages after the current page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Indicates if there are pages before the current page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    public PaginatedResult(IReadOnlyCollection<T> items, int pageNumber, int pageSize, int totalItems)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}

/// <summary>
/// Query parameters for paginated list requests.
/// Provides standard paging controls for all list endpoints.
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Current page number (1-based). Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10, maximum 100.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Search term to filter results (optional).
    /// Implementation-specific; used by endpoints that support searching.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Comma-separated field names for sorting (optional).
    /// Prefix with '-' for descending order, e.g., "name,-createdAt"
    /// </summary>
    public string? SortBy { get; set; }
}

namespace BenBristow.EntityFrameworkCore.Pagination.Models;

/// <summary>
/// Represents a paginated result set of items of type T.
/// </summary>
/// <typeparam name="T">The type of items in the result set.</typeparam>
public class PaginationResult<T>
    where T : class
{
    /// <summary>
    /// The collection of items for the current page.
    /// </summary>
    public required ICollection<T> Results { get; init; }

    /// <summary>
    /// The total count of items across all pages.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// The current page number.
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public required int? PageSize { get; init; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public required int PageCount { get; init; }
}
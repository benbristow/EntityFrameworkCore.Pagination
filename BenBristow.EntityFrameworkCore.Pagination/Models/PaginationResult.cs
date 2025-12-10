using System.Collections;

namespace BenBristow.EntityFrameworkCore.Pagination.Models;

/// <summary>
/// Non-generic base class for paginated result sets. Useful as a Razor Pages model.
/// </summary>
public abstract class PaginationResult
{
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

    /// <summary>
    /// Non-generic access to the current page results.
    /// </summary>
    public abstract ICollection ResultsUntyped { get; }
}

/// <summary>
/// Represents a paginated result set of items of type T.
/// </summary>
/// <typeparam name="T">The type of items in the result set.</typeparam>
public class PaginationResult<T> : PaginationResult
    where T : class
{
    /// <summary>
    /// The collection of items for the current page.
    /// </summary>
    public required ICollection<T> Results { get; init; }

    /// <inheritdoc />
    public override ICollection ResultsUntyped => (ICollection)Results;

    /// <summary>
    /// Creates an empty pagination result with no items.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>An empty <see cref="PaginationResult{T}"/> instance.</returns>
    public static PaginationResult<T> Empty(int? pageSize = null)
    {
        return new PaginationResult<T>
        {
            Results = Array.Empty<T>(),
            TotalCount = 0,
            Page = 1,
            PageCount = 1,
            PageSize = pageSize
        };
    }
}
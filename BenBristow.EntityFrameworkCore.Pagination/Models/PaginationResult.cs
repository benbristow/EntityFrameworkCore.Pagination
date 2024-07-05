namespace BenBristow.EntityFrameworkCore.Pagination.Models;

public class PaginationResult<T>
    where T : class
{
    public required ICollection<T> Results { get; init; }
    public required int TotalCount { get; init; }
    public required int Page { get; init; }
    public required int? PageSize { get; init; }
    public required int PageCount { get; init; }
}

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using BenBristow.EntityFrameworkCore.Pagination.Models;
using Microsoft.EntityFrameworkCore;

namespace BenBristow.EntityFrameworkCore.Pagination.Extensions;

/// <summary>
/// Provides extension methods for IQueryable to support pagination functionality.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Asynchronously paginates the queryable source based on the provided page and pageSize parameters.
    /// If pageSize is null, all results are returned.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source.</typeparam>
    /// <param name="source">The ordered queryable source to paginate.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to <see cref="Constants.DefaultPageSize"/>.
    /// If null, all items are returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated results.</returns>
    public static async Task<PaginationResult<T>> ToPaginationResultAsync<T>(
        this IOrderedQueryable<T> source,
        int page = 1,
        int? pageSize = Constants.DefaultPageSize,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (pageSize is null)
            return await GetAllResultsAsync(source, cancellationToken);

        return await PaginateResultsAsync(source, page, pageSize, cancellationToken);
    }

    /// <summary>
    /// Asynchronously paginates the queryable source based on the provided page and pageSize parameters,
    /// applying a projection before pagination.
    /// If pageSize is null, all results are returned.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source.</typeparam>
    /// <typeparam name="TResult">The type of the elements after projection.</typeparam>
    /// <param name="source">The ordered queryable source to paginate.</param>
    /// <param name="projection">A function to map each element of TSource to TResult.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to <see cref="Constants.DefaultPageSize"/>.
    /// If null, all items are returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated results after projection.</returns>
    public static async Task<PaginationResult<TResult>> ToPaginationResultAsync<TSource, TResult>(
        this IOrderedQueryable<TSource> source,
        Expression<Func<TSource, TResult>> projection,
        int page = 1,
        int? pageSize = Constants.DefaultPageSize,
        CancellationToken cancellationToken = default)
        where TSource : class
        where TResult : class
    {
        if (pageSize is null)
            return await GetAllResultsAsync(source, projection, cancellationToken);

        return await PaginateResultsAsync(source, projection, page, pageSize, cancellationToken);
    }

    private static async Task<PaginationResult<T>> PaginateResultsAsync<T>(
        IOrderedQueryable<T> source,
        int page,
        [DisallowNull] int? pageSize,
        CancellationToken cancellationToken) where T : class
    {
        var count = await source.AsNoTracking().CountAsync(cancellationToken);
        var results = await GetResultsQuery(source: source.AsNoTracking(), page: page, pageSize: pageSize.Value, totalCount: count)
            .ToListAsync(cancellationToken);
        var pageCount = (int)Math.Ceiling(count / (double)pageSize.Value);

        return new PaginationResult<T>
        {
            Results = results,
            TotalCount = count,
            Page = page > pageCount ? pageCount : page,
            PageCount = pageCount < 1 ? 1 : pageCount,
            PageSize = pageSize,
        };
    }

    private static async Task<PaginationResult<TResult>> PaginateResultsAsync<TSource, TResult>(
        IOrderedQueryable<TSource> source,
        Expression<Func<TSource, TResult>> projection,
        int page,
        [DisallowNull] int? pageSize,
        CancellationToken cancellationToken)
        where TSource : class
        where TResult : class
    {
        var count = await source.AsNoTracking().CountAsync(cancellationToken);
        var results = await GetResultsQuery(source: source.AsNoTracking(), page: page, pageSize: pageSize.Value, totalCount: count)
            .Select(projection)
            .ToListAsync(cancellationToken);
        var pageCount = (int)Math.Ceiling(count / (double)pageSize.Value);

        return new PaginationResult<TResult>
        {
            Results = results,
            TotalCount = count,
            Page = page > pageCount ? pageCount : page,
            PageCount = pageCount < 1 ? 1 : pageCount,
            PageSize = pageSize,
        };
    }

    private static async Task<PaginationResult<T>> GetAllResultsAsync<T>(
        IOrderedQueryable<T> source,
        CancellationToken cancellationToken)
        where T : class
    {
        var allResults = await source.AsNoTracking().ToListAsync(cancellationToken);

        return new PaginationResult<T>
        {
            Results = allResults,
            TotalCount = allResults.Count,
            Page = 1,
            PageCount = 1,
            PageSize = null,
        };
    }

    private static async Task<PaginationResult<TResult>> GetAllResultsAsync<TSource, TResult>(
        IOrderedQueryable<TSource> source,
        Expression<Func<TSource, TResult>> projection,
        CancellationToken cancellationToken)
        where TSource : class
        where TResult : class
    {
        var allResults = await source.AsNoTracking().Select(projection).ToListAsync(cancellationToken);

        return new PaginationResult<TResult>
        {
            Results = allResults,
            TotalCount = allResults.Count,
            Page = 1,
            PageCount = 1,
            PageSize = null,
        };
    }

    private static IQueryable<T> GetResultsQuery<T>(IQueryable<T> source, int page, int pageSize, int totalCount)
    {
        var resultsQuery = source.AsQueryable();
        if (totalCount >= pageSize) // Only apply pagination if there are enough results
            resultsQuery = resultsQuery.Skip((page - 1) * pageSize).Take(pageSize);
        return resultsQuery;
    }
}
using Microsoft.EntityFrameworkCore;
using ViewForge.Builders;
using ViewForge.Models;

namespace ViewForge.Extensions;

/// <summary>
/// Extension methods for IQueryable to apply ViewForge filters, sorting, and pagination.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies a filter group to the queryable using the specified filter builder.
    /// </summary>
    public static IQueryable<T> ApplyFilters<T>(
        this IQueryable<T> query,
        FilterGroup? filterGroup,
        IFilterBuilder filterBuilder) where T : class
    {
        if (filterGroup is null)
            return query;

        var expression = filterBuilder.Build<T>(filterGroup);
        if (expression is null)
            return query;

        return query.Where(expression);
    }

    /// <summary>
    /// Applies sort descriptors to the queryable using the specified sort builder.
    /// </summary>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        IEnumerable<SortDescriptor> sortDescriptors,
        ISortBuilder sortBuilder) where T : class
    {
        return sortBuilder.ApplySort(query, sortDescriptors);
    }

    /// <summary>
    /// Converts a queryable to a paginated result with total count.
    /// Page and pageSize are fully parameterized — pass them from controller parameters,
    /// query strings, or any external source.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="query">The source queryable (already filtered and sorted).</param>
    /// <param name="page">The page number (1-based). Pass from external source.</param>
    /// <param name="pageSize">The number of items per page. Pass from external source.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated result with items and metadata.</returns>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) where T : class
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Converts a queryable to a list result without pagination.
    /// </summary>
    public static async Task<PagedResult<T>> ToUnpagedResultAsync<T>(
        this IQueryable<T> query,
        CancellationToken cancellationToken = default) where T : class
    {
        var items = await query.ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = items.Count,
            Page = 1,
            PageSize = items.Count
        };
    }
}

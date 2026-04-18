using Microsoft.EntityFrameworkCore;
using ViewForge.Builders;
using ViewForge.Configuration;
using ViewForge.Extensions;
using ViewForge.Models;

namespace ViewForge.Providers;

/// <summary>
/// EF Core implementation of <see cref="IViewQueryProvider"/>.
/// Queries DbSet-registered views using LINQ with expression tree-based
/// dynamic filtering, sorting, and pagination.
/// </summary>
public class EfCoreViewQueryProvider : IViewQueryProvider
{
    private readonly DbContext _dbContext;
    private readonly IFilterBuilder _filterBuilder;
    private readonly ISortBuilder _sortBuilder;
    private readonly ViewForgeOptions _options;

    /// <summary>
    /// Creates a new EfCoreViewQueryProvider.
    /// </summary>
    /// <param name="dbContext">The EF Core DbContext containing the view DbSets.</param>
    /// <param name="filterBuilder">The filter builder for expression generation.</param>
    /// <param name="sortBuilder">The sort builder for ordering.</param>
    /// <param name="options">ViewForge configuration options.</param>
    public EfCoreViewQueryProvider(
        DbContext dbContext,
        IFilterBuilder filterBuilder,
        ISortBuilder sortBuilder,
        ViewForgeOptions options)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _filterBuilder = filterBuilder ?? throw new ArgumentNullException(nameof(filterBuilder));
        _sortBuilder = sortBuilder ?? throw new ArgumentNullException(nameof(sortBuilder));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public async Task<PagedResult<T>> QueryAsync<T>(
        ViewQueryRequest request,
        CancellationToken cancellationToken = default) where T : class, new()
    {
        var query = BuildQuery<T>(request);

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Min(
            request.PageSize < 1 ? _options.DefaultPageSize : request.PageSize,
            _options.MaxPageSize);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<T>> QueryListAsync<T>(
        ViewQueryRequest request,
        CancellationToken cancellationToken = default) where T : class, new()
    {
        var query = BuildQuery<T>(request);
        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> CountAsync<T>(
        ViewQueryRequest request,
        CancellationToken cancellationToken = default) where T : class, new()
    {
        var query = GetBaseQuery<T>();
        query = query.ApplyFilters(request.Filters, _filterBuilder);
        return await query.LongCountAsync(cancellationToken);
    }

    private IQueryable<T> BuildQuery<T>(ViewQueryRequest request) where T : class, new()
    {
        var query = GetBaseQuery<T>();

        // Apply filters
        query = query.ApplyFilters(request.Filters, _filterBuilder);

        // Apply sorting
        if (request.Sorting.Count > 0)
        {
            query = query.ApplySorting(request.Sorting, _sortBuilder);
        }

        return query;
    }

    private IQueryable<T> GetBaseQuery<T>() where T : class, new()
    {
        return _dbContext.Set<T>().AsNoTracking();
    }
}

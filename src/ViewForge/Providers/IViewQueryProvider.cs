using ViewForge.Models;

namespace ViewForge.Providers;

/// <summary>
/// Defines the contract for querying SQL views with dynamic filtering, sorting, and pagination.
/// Implementations follow the Strategy pattern — swap between EF Core, ADO.NET, etc.
/// </summary>
public interface IViewQueryProvider
{
    /// <summary>
    /// Queries a SQL view with filters, sorting, and pagination.
    /// Returns a paginated result with total count.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="request">The query request containing filters, sorting, and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated result.</returns>
    Task<PagedResult<T>> QueryAsync<T>(ViewQueryRequest request, CancellationToken cancellationToken = default)
        where T : class, new();

    /// <summary>
    /// Queries a SQL view with filters and sorting, returning all results without pagination.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="request">The query request containing filters and sorting.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching items.</returns>
    Task<List<T>> QueryListAsync<T>(ViewQueryRequest request, CancellationToken cancellationToken = default)
        where T : class, new();

    /// <summary>
    /// Returns the total count of records matching the filters.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="request">The query request containing filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total count.</returns>
    Task<long> CountAsync<T>(ViewQueryRequest request, CancellationToken cancellationToken = default)
        where T : class, new();
}

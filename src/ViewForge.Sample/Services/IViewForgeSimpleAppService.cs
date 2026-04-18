using ViewForge.Models;
using ViewForge.Sample.Views;

namespace ViewForge.Sample.Services;

/// <summary>
/// Application service interface for product view operations.
/// </summary>
public interface IViewForgeSimpleAppService
{
    Task<PagedResult<ProductSummaryView>> GetFilteredListAsync(
        string? filters,
        string? sorting,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductSummaryView?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

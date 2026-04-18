using ViewForge.Models;
using ViewForge.Sample.Views;

namespace ViewForge.Sample.Repositories;

/// <summary>
/// Repository interface for product summary view data access.
/// </summary>
public interface IViewForgeSimpleRepository
{
    Task<PagedResult<ProductSummaryView>> GetFilteredListAsync(
        FilterGroup? filterGroup,
        List<SortDescriptor> sortDescriptors,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductSummaryView?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

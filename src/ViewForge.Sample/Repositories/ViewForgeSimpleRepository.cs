using Microsoft.EntityFrameworkCore;
using ViewForge.Builders;
using ViewForge.Extensions;
using ViewForge.Models;
using ViewForge.Sample.Data;
using ViewForge.Sample.Views;

namespace ViewForge.Sample.Repositories;

/// <summary>
/// EF Core repository implementation for product summary view.
/// All database/ViewForge query operations happen here — nowhere else.
/// </summary>
public class ViewForgeSimpleRepository : IViewForgeSimpleRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IFilterBuilder _filterBuilder;
    private readonly ISortBuilder _sortBuilder;

    public ViewForgeSimpleRepository(
        AppDbContext dbContext,
        IFilterBuilder filterBuilder,
        ISortBuilder sortBuilder)
    {
        _dbContext = dbContext;
        _filterBuilder = filterBuilder;
        _sortBuilder = sortBuilder;
    }

    public async Task<PagedResult<ProductSummaryView>> GetFilteredListAsync(
        FilterGroup? filterGroup,
        List<SortDescriptor> sortDescriptors,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ProductSummaries.AsNoTracking().AsQueryable();

        query = query.ApplyFilters(filterGroup, _filterBuilder);
        query = query.ApplySorting(sortDescriptors, _sortBuilder);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    public async Task<ProductSummaryView?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductSummaries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}

using ViewForge.Builders;
using ViewForge.Models;
using ViewForge.Sample.Repositories;
using ViewForge.Sample.Views;

namespace ViewForge.Sample.Services;

/// <summary>
/// Application service implementation for product view operations.
/// Orchestrates repository and ViewForge builders — no DB logic here.
/// </summary>
public class ViewForgeSimpleAppService : IViewForgeSimpleAppService
{
    private readonly IViewForgeSimpleRepository _repository;
    private readonly FilterParser _filterParser;

    public ViewForgeSimpleAppService(
        IViewForgeSimpleRepository repository,
        FilterParser filterParser)
    {
        _repository = repository;
        _filterParser = filterParser;
    }

    public async Task<PagedResult<ProductSummaryView>> GetFilteredListAsync(
        string? filters,
        string? sorting,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filterGroup = _filterParser.Parse(filters);
        var sortDescriptors = SortBuilder.Parse(sorting);

        return await _repository.GetFilteredListAsync(
            filterGroup, sortDescriptors, page, pageSize, cancellationToken);
    }

    public async Task<ProductSummaryView?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }
}

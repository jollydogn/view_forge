using Microsoft.AspNetCore.Mvc;
using ViewForge.Builders;
using ViewForge.Configuration;
using ViewForge.Extensions;
using ViewForge.Models;
using ViewForge.Sample.Data;
using ViewForge.Sample.Views;

namespace ViewForge.Sample.Controllers;

/// <summary>
/// Demonstrates ViewForge dynamic filtering, sorting, and pagination.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IFilterBuilder _filterBuilder;
    private readonly ISortBuilder _sortBuilder;
    private readonly FilterParser _filterParser;
    private readonly ViewForgeOptions _options;

    public ProductsController(
        AppDbContext dbContext,
        IFilterBuilder filterBuilder,
        ISortBuilder sortBuilder,
        FilterParser filterParser,
        ViewForgeOptions options)
    {
        _dbContext = dbContext;
        _filterBuilder = filterBuilder;
        _sortBuilder = sortBuilder;
        _filterParser = filterParser;
        _options = options;
    }

    /// <summary>
    /// Get products with dynamic filtering, sorting, and pagination.
    /// Filter format: PropertyName~operator~value, separated by ";"
    /// Sorting format: "PropertyName desc, PropertyName2 asc"
    /// </summary>
    /// <remarks>
    /// Example filters:
    /// - ProductName~contains~keyboard
    /// - Price~gt~100
    /// - IsActive~eq~true
    /// - SupplierId~isnull
    /// - CreatedDate~between~2024-01-01,2024-06-30
    /// - CategoryName~in~Electronics,Accessories
    /// </remarks>
    [HttpGet]
    public async Task<PagedResult<ProductSummaryView>> GetProducts(
        [FromQuery] string? filters = null,
        [FromQuery] string? sorting = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var filterGroup = _filterParser.Parse(filters);
        var sortDescriptors = SortBuilder.Parse(sorting);

        var query = _dbContext.ProductSummaries.AsQueryable();

        query = query.ApplyFilters(filterGroup, _filterBuilder);
        query = query.ApplySorting(sortDescriptors, _sortBuilder);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    /// <summary>
    /// Get a single product by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductSummaryView>> GetProduct(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.ProductSummaries.FindAsync(new object[] { id }, cancellationToken);
        if (product is null)
            return NotFound();

        return Ok(product);
    }
}

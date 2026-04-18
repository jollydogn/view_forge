using Microsoft.AspNetCore.Mvc;
using ViewForge.Models;
using ViewForge.Sample.Services;
using ViewForge.Sample.Views;

namespace ViewForge.Sample.Controllers;

/// <summary>
/// Demonstrates ViewForge integration with proper layered architecture.
/// Controller only receives requests and delegates to AppService.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IViewForgeSimpleAppService _appService;

    public ProductsController(IViewForgeSimpleAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// Get products with dynamic filtering, sorting, and pagination.
    /// </summary>
    [HttpGet]
    public async Task<PagedResult<ProductSummaryView>> GetListAsync(
        [FromQuery] string? filters = null,
        [FromQuery] string? sorting = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _appService.GetFilteredListAsync(filters, sorting, page, pageSize, cancellationToken);
    }

    /// <summary>
    /// Get a single product by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductSummaryView>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await _appService.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return NotFound();

        return Ok(product);
    }
}

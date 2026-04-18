using ViewForge.Attributes;

namespace ViewForge.Sample.Views;

/// <summary>
/// Represents a product summary view model mapped to a SQL view.
/// Demonstrates ViewForge attribute-based configuration.
/// </summary>
[ViewName("vw_product_summary")]
public class ProductSummaryView
{
    public Guid Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? CategoryName { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public decimal TotalSales { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? SupplierId { get; set; }

    public string? SupplierName { get; set; }

    [ViewIgnore]
    public string DisplayName => $"{ProductName} ({CategoryName ?? "Uncategorized"})";
}

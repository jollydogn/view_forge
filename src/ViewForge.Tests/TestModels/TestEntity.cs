using ViewForge.Attributes;

namespace ViewForge.Tests.TestModels;

[ViewName("vw_test_entity")]
public class TestEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public int? NullableQuantity { get; set; }
    public decimal Price { get; set; }
    public decimal? NullablePrice { get; set; }
    public double Rating { get; set; }
    public bool IsActive { get; set; }
    public bool? NullableIsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? NullableDate { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? NullableCategoryId { get; set; }

    [ViewIgnore]
    public string Computed => $"{Name}_{Quantity}";

    [ViewColumn("custom_col")]
    public string MappedColumn { get; set; } = string.Empty;
}

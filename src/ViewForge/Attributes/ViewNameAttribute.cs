namespace ViewForge.Attributes;

/// <summary>
/// Maps a class to a specific SQL view name in the database.
/// If not specified, ViewForge will attempt to derive the view name from the class name.
/// </summary>
/// <example>
/// <code>
/// [ViewName("vw_product_summary")]
/// public class ProductSummaryView { ... }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ViewNameAttribute : Attribute
{
    /// <summary>
    /// The SQL view name in the database.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Optional schema name (e.g., "dbo", "sales").
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// Creates a new ViewNameAttribute with the specified view name.
    /// </summary>
    /// <param name="name">The SQL view name in the database.</param>
    public ViewNameAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

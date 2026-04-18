using ViewForge.Enums;

namespace ViewForge.Models;

/// <summary>
/// Describes a sort operation on a single property.
/// </summary>
public class SortDescriptor
{
    /// <summary>
    /// The name of the property to sort by (must match the C# model property name).
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// The sort direction (Ascending or Descending).
    /// </summary>
    public SortDirection Direction { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Creates a new SortDescriptor for the specified property.
    /// </summary>
    public static SortDescriptor Create(string propertyName, SortDirection direction = SortDirection.Ascending)
    {
        return new SortDescriptor
        {
            PropertyName = propertyName,
            Direction = direction
        };
    }
}

using ViewForge.Enums;

namespace ViewForge.Models;

/// <summary>
/// Describes a single filter condition to be applied on a property.
/// Supports all primitive types, DateTime, Guid, and their nullable variants.
/// </summary>
public class FilterDescriptor
{
    /// <summary>
    /// The name of the property to filter on (must match the C# model property name).
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// The comparison operator to use (Equals, Contains, GreaterThan, etc.).
    /// </summary>
    public FilterOperator Operator { get; set; } = FilterOperator.Equals;

    /// <summary>
    /// The value to compare against. Null for IsNull/IsNotNull operators.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// The upper bound value for Between operator. Ignored for other operators.
    /// </summary>
    public object? ValueTo { get; set; }

    /// <summary>
    /// Creates a new FilterDescriptor with the specified parameters.
    /// </summary>
    public static FilterDescriptor Create(string propertyName, FilterOperator op, object? value = null, object? valueTo = null)
    {
        return new FilterDescriptor
        {
            PropertyName = propertyName,
            Operator = op,
            Value = value,
            ValueTo = valueTo
        };
    }
}

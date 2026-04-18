using ViewForge.Enums;

namespace ViewForge.Attributes;

/// <summary>
/// Marks a property as filterable, optionally restricting which operators can be used.
/// If ViewForgeOptions.AllPropertiesFilterableByDefault is true, this attribute
/// can be used to restrict operators. If false, only properties marked with this
/// attribute will be filterable.
/// </summary>
/// <example>
/// <code>
/// [Filterable(AllowedOperators = new[] { FilterOperator.Equals, FilterOperator.Contains })]
/// public string ProductName { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class FilterableAttribute : Attribute
{
    /// <summary>
    /// The operators allowed for this property. If null or empty, all operators are allowed.
    /// </summary>
    public FilterOperator[]? AllowedOperators { get; set; }
}

namespace ViewForge.Attributes;

/// <summary>
/// Marks a property as sortable.
/// If ViewForgeOptions.AllPropertiesSortableByDefault is true, all properties
/// are sortable by default and this attribute is not required.
/// If false, only properties marked with this attribute can be sorted.
/// </summary>
/// <example>
/// <code>
/// [Sortable]
/// public DateTime CreatedDate { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SortableAttribute : Attribute
{
}

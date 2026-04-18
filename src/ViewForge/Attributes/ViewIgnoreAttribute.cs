namespace ViewForge.Attributes;

/// <summary>
/// Excludes a property from view column mapping and filter/sort operations.
/// Useful for computed properties or navigation properties that don't exist in the view.
/// </summary>
/// <example>
/// <code>
/// [ViewIgnore]
/// public string FullName => $"{FirstName} {LastName}";
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ViewIgnoreAttribute : Attribute
{
}

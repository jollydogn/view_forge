using System.Reflection;

namespace ViewForge.Mapping;

/// <summary>
/// Defines the contract for mapping between C# model properties and SQL view columns.
/// </summary>
public interface IViewMapper
{
    /// <summary>
    /// Gets the SQL view name for the specified model type.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <returns>The SQL view name.</returns>
    string GetViewName<T>() where T : class;

    /// <summary>
    /// Gets the SQL view name for the specified model type.
    /// </summary>
    /// <param name="type">The model type.</param>
    /// <returns>The SQL view name.</returns>
    string GetViewName(Type type);

    /// <summary>
    /// Gets the SQL column name for the specified property.
    /// </summary>
    /// <param name="property">The property info.</param>
    /// <returns>The SQL column name.</returns>
    string GetColumnName(PropertyInfo property);

    /// <summary>
    /// Gets the property info for the specified column or property name.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="name">The property or column name.</param>
    /// <returns>The property info, or null if not found.</returns>
    PropertyInfo? GetProperty<T>(string name) where T : class;

    /// <summary>
    /// Gets all mappable properties for the specified model type (excludes [ViewIgnore] properties).
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <returns>A list of mappable property infos.</returns>
    IReadOnlyList<PropertyInfo> GetMappableProperties<T>() where T : class;
}

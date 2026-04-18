using System.Collections.Concurrent;
using System.Reflection;
using ViewForge.Attributes;
using ViewForge.Configuration;
using ViewForge.Enums;

namespace ViewForge.Mapping;

/// <summary>
/// Default implementation of <see cref="IViewMapper"/>.
/// Maps C# model properties to SQL view columns using attributes and naming conventions.
/// Results are cached for performance.
/// </summary>
public class ViewMapper : IViewMapper
{
    private readonly ViewForgeOptions _options;
    private readonly ConcurrentDictionary<Type, string> _viewNameCache = new();
    private readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> _propertyCache = new();
    private readonly ConcurrentDictionary<string, string> _columnNameCache = new();

    /// <summary>
    /// Creates a new ViewMapper with the specified options.
    /// </summary>
    public ViewMapper(ViewForgeOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public string GetViewName<T>() where T : class => GetViewName(typeof(T));

    /// <inheritdoc />
    public string GetViewName(Type type)
    {
        return _viewNameCache.GetOrAdd(type, t =>
        {
            var attr = t.GetCustomAttribute<ViewNameAttribute>();
            if (attr is not null)
            {
                return attr.Schema is not null
                    ? $"{attr.Schema}.{attr.Name}"
                    : attr.Name;
            }

            // Derive from class name using naming convention
            return NamingConventionHelper.Convert(t.Name, _options.DefaultNamingConvention);
        });
    }

    /// <inheritdoc />
    public string GetColumnName(PropertyInfo property)
    {
        var cacheKey = $"{property.DeclaringType?.FullName}.{property.Name}";
        return _columnNameCache.GetOrAdd(cacheKey, _ =>
        {
            var attr = property.GetCustomAttribute<ViewColumnAttribute>();
            return attr?.ColumnName
                   ?? NamingConventionHelper.Convert(property.Name, _options.DefaultNamingConvention);
        });
    }

    /// <inheritdoc />
    public PropertyInfo? GetProperty<T>(string name) where T : class
    {
        var properties = GetMappableProperties<T>();

        // Direct property name match (case-insensitive)
        var directMatch = properties.FirstOrDefault(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        if (directMatch is not null)
            return directMatch;

        // Column name match
        var columnMatch = properties.FirstOrDefault(p =>
            string.Equals(GetColumnName(p), name, StringComparison.OrdinalIgnoreCase));
        if (columnMatch is not null)
            return columnMatch;

        // Fuzzy naming convention match
        return properties.FirstOrDefault(p =>
            NamingConventionHelper.NamesMatch(p.Name, name));
    }

    /// <inheritdoc />
    public IReadOnlyList<PropertyInfo> GetMappableProperties<T>() where T : class
    {
        return _propertyCache.GetOrAdd(typeof(T), t =>
        {
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<ViewIgnoreAttribute>() is null)
                .Where(p => p.CanRead)
                .ToList()
                .AsReadOnly();
        });
    }
}

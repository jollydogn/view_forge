using System.Linq.Expressions;
using ViewForge.Configuration;
using ViewForge.Enums;
using ViewForge.Mapping;
using ViewForge.Models;

namespace ViewForge.Builders;

/// <summary>
/// Builds dynamic sort expressions from sort descriptors using expression trees.
/// </summary>
public class SortBuilder : ISortBuilder
{
    private readonly IViewMapper _mapper;
    private readonly ViewForgeOptions _options;

    /// <summary>
    /// Creates a new SortBuilder with the specified dependencies.
    /// </summary>
    public SortBuilder(IViewMapper mapper, ViewForgeOptions options)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public IQueryable<T> ApplySort<T>(IQueryable<T> query, IEnumerable<SortDescriptor> sortDescriptors) where T : class
    {
        var descriptors = sortDescriptors.ToList();
        if (descriptors.Count == 0)
            return query;

        IOrderedQueryable<T>? orderedQuery = null;

        for (var i = 0; i < descriptors.Count; i++)
        {
            var descriptor = descriptors[i];
            var property = _mapper.GetProperty<T>(descriptor.PropertyName);

            if (property is null)
            {
                throw new InvalidOperationException(
                    $"Sort property '{descriptor.PropertyName}' not found on type '{typeof(T).Name}'. " +
                    $"Ensure the property exists and is not marked with [ViewIgnore].");
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = GetOrderMethodName(descriptor.Direction, isFirst: i == 0);

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType);

            if (i == 0)
            {
                orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
            }
            else
            {
                orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQuery!, lambda })!;
            }
        }

        return orderedQuery ?? query;
    }

    /// <summary>
    /// Parses a sorting string into sort descriptors.
    /// Format: "PropertyName desc, PropertyName2 asc" or "PropertyName,-PropertyName2"
    /// </summary>
    public static List<SortDescriptor> Parse(string? sorting)
    {
        var result = new List<SortDescriptor>();
        if (string.IsNullOrWhiteSpace(sorting))
            return result;

        var parts = sorting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            // Support "-PropertyName" syntax for descending
            if (part.StartsWith('-'))
            {
                result.Add(SortDescriptor.Create(part[1..], SortDirection.Descending));
                continue;
            }

            // Support "PropertyName desc" syntax
            var segments = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2)
            {
                var direction = segments[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? SortDirection.Descending
                    : SortDirection.Ascending;
                result.Add(SortDescriptor.Create(segments[0], direction));
            }
            else
            {
                result.Add(SortDescriptor.Create(segments[0], SortDirection.Ascending));
            }
        }

        return result;
    }

    private static string GetOrderMethodName(SortDirection direction, bool isFirst)
    {
        return (direction, isFirst) switch
        {
            (SortDirection.Ascending, true) => nameof(Queryable.OrderBy),
            (SortDirection.Descending, true) => nameof(Queryable.OrderByDescending),
            (SortDirection.Ascending, false) => nameof(Queryable.ThenBy),
            (SortDirection.Descending, false) => nameof(Queryable.ThenByDescending),
            _ => nameof(Queryable.OrderBy)
        };
    }
}

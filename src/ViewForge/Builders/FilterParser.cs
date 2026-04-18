using ViewForge.Configuration;
using ViewForge.Enums;
using ViewForge.Models;

namespace ViewForge.Builders;

/// <summary>
/// Parses string-based filter expressions from query strings into FilterDescriptor objects.
/// Format: PropertyName~operator~value (e.g., "ProductName~contains~keyboard")
/// Multiple filters separated by ";" (e.g., "Name~contains~test;Price~gt~100")
/// </summary>
public class FilterParser
{
    private readonly ViewForgeOptions _options;

    private static readonly Dictionary<string, FilterOperator> OperatorAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["eq"] = FilterOperator.Equals,
        ["equals"] = FilterOperator.Equals,
        ["neq"] = FilterOperator.NotEquals,
        ["notequals"] = FilterOperator.NotEquals,
        ["contains"] = FilterOperator.Contains,
        ["startswith"] = FilterOperator.StartsWith,
        ["endswith"] = FilterOperator.EndsWith,
        ["gt"] = FilterOperator.GreaterThan,
        ["greaterthan"] = FilterOperator.GreaterThan,
        ["gte"] = FilterOperator.GreaterThanOrEqual,
        ["lt"] = FilterOperator.LessThan,
        ["lessthan"] = FilterOperator.LessThan,
        ["lte"] = FilterOperator.LessThanOrEqual,
        ["in"] = FilterOperator.In,
        ["isnull"] = FilterOperator.IsNull,
        ["null"] = FilterOperator.IsNull,
        ["isnotnull"] = FilterOperator.IsNotNull,
        ["notnull"] = FilterOperator.IsNotNull,
        ["between"] = FilterOperator.Between,
        ["btw"] = FilterOperator.Between
    };

    /// <summary>
    /// Creates a new FilterParser with the specified options.
    /// </summary>
    public FilterParser(ViewForgeOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Parses a filter string into a FilterGroup.
    /// </summary>
    /// <param name="filterString">The filter string (e.g., "Name~contains~test;Price~gt~100").</param>
    /// <returns>A FilterGroup containing parsed filters, or null if input is empty.</returns>
    public FilterGroup? Parse(string? filterString)
    {
        if (string.IsNullOrWhiteSpace(filterString))
            return null;

        var filters = new List<FilterDescriptor>();
        var parts = filterString.Split(_options.FilterDelimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            var descriptor = ParseSingle(part);
            if (descriptor is not null)
            {
                filters.Add(descriptor);
            }
        }

        if (filters.Count == 0)
            return null;

        return new FilterGroup
        {
            Logic = LogicalOperator.And,
            Filters = filters
        };
    }

    /// <summary>
    /// Parses a single filter expression (e.g., "ProductName~contains~keyboard").
    /// </summary>
    private FilterDescriptor? ParseSingle(string expression)
    {
        var separator = _options.FilterSeparator;
        var segments = expression.Split(separator, StringSplitOptions.None);

        if (segments.Length < 2)
            return null;

        var propertyName = segments[0].Trim();
        var operatorAlias = segments[1].Trim();

        if (!OperatorAliases.TryGetValue(operatorAlias, out var filterOperator))
            return null;

        // IsNull and IsNotNull don't need a value
        if (filterOperator is FilterOperator.IsNull or FilterOperator.IsNotNull)
        {
            return FilterDescriptor.Create(propertyName, filterOperator);
        }

        if (segments.Length < 3)
            return null;

        var value = segments[2].Trim();

        // Between operator: value format is "from,to"
        if (filterOperator == FilterOperator.Between)
        {
            var betweenParts = value.Split(',', 2);
            if (betweenParts.Length == 2)
            {
                return FilterDescriptor.Create(
                    propertyName, filterOperator,
                    betweenParts[0].Trim(),
                    betweenParts[1].Trim());
            }
        }

        return FilterDescriptor.Create(propertyName, filterOperator, value);
    }
}

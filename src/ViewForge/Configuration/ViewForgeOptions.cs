using ViewForge.Enums;

namespace ViewForge.Configuration;

/// <summary>
/// Global configuration options for ViewForge.
/// Configure via <c>builder.Services.AddViewForge(options => { ... })</c>.
/// </summary>
public class ViewForgeOptions
{
    /// <summary>
    /// The default naming convention for mapping property names to column names.
    /// Defaults to <see cref="NamingConvention.SnakeCase"/>.
    /// </summary>
    public NamingConvention DefaultNamingConvention { get; set; } = NamingConvention.SnakeCase;

    /// <summary>
    /// When true, all properties are filterable by default unless excluded with [ViewIgnore].
    /// When false, only properties marked with [Filterable] can be filtered.
    /// Defaults to true.
    /// </summary>
    public bool AllPropertiesFilterableByDefault { get; set; } = true;

    /// <summary>
    /// When true, all properties are sortable by default unless excluded with [ViewIgnore].
    /// When false, only properties marked with [Sortable] can be sorted.
    /// Defaults to true.
    /// </summary>
    public bool AllPropertiesSortableByDefault { get; set; } = true;

    /// <summary>
    /// When true, string filter operations (Contains, StartsWith, EndsWith) are case-insensitive.
    /// Defaults to true.
    /// </summary>
    public bool CaseInsensitiveFilters { get; set; } = true;

    /// <summary>
    /// The default page size when pagination is not explicitly specified.
    /// Defaults to 20.
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// The maximum allowed page size to prevent excessive data retrieval.
    /// Defaults to 100.
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// The separator used in string-based filter expressions.
    /// Defaults to "~" (e.g., "PropertyName~contains~value").
    /// </summary>
    public string FilterSeparator { get; set; } = "~";

    /// <summary>
    /// The delimiter used to separate multiple filters in a query string.
    /// Defaults to ";" (e.g., "filter1;filter2;filter3").
    /// </summary>
    public string FilterDelimiter { get; set; } = ";";
}

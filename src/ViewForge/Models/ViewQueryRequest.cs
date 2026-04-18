namespace ViewForge.Models;

/// <summary>
/// Combines filter, sort, and pagination parameters into a single request object.
/// Can be constructed programmatically or parsed from query string parameters.
/// </summary>
public class ViewQueryRequest
{
    /// <summary>
    /// The filter group containing all filter conditions.
    /// </summary>
    public FilterGroup? Filters { get; set; }

    /// <summary>
    /// The list of sort descriptors to apply (in order of priority).
    /// </summary>
    public List<SortDescriptor> Sorting { get; set; } = new();

    /// <summary>
    /// The page number to retrieve (1-based). Defaults to 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items per page. Defaults to 20.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Whether to include the total count in the response (may add a COUNT query).
    /// Defaults to true.
    /// </summary>
    public bool IncludeTotalCount { get; set; } = true;
}

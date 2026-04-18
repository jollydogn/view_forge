using ViewForge.Enums;

namespace ViewForge.Models;

/// <summary>
/// Groups multiple filter conditions with a logical operator (AND/OR).
/// Supports nested groups for complex filter expressions.
/// </summary>
public class FilterGroup
{
    /// <summary>
    /// The logical operator to combine the filters within this group.
    /// </summary>
    public LogicalOperator Logic { get; set; } = LogicalOperator.And;

    /// <summary>
    /// The list of individual filter conditions in this group.
    /// </summary>
    public List<FilterDescriptor> Filters { get; set; } = new();

    /// <summary>
    /// Nested filter groups for complex logical expressions (e.g., (A AND B) OR (C AND D)).
    /// </summary>
    public List<FilterGroup> Groups { get; set; } = new();

    /// <summary>
    /// Creates an AND group with the specified filters.
    /// </summary>
    public static FilterGroup And(params FilterDescriptor[] filters)
    {
        return new FilterGroup
        {
            Logic = LogicalOperator.And,
            Filters = filters.ToList()
        };
    }

    /// <summary>
    /// Creates an OR group with the specified filters.
    /// </summary>
    public static FilterGroup Or(params FilterDescriptor[] filters)
    {
        return new FilterGroup
        {
            Logic = LogicalOperator.Or,
            Filters = filters.ToList()
        };
    }
}

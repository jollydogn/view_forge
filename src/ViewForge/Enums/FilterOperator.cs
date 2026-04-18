namespace ViewForge.Enums;

/// <summary>
/// Defines the comparison operators available for dynamic filtering.
/// </summary>
public enum FilterOperator
{
    /// <summary>Exact equality comparison.</summary>
    Equals = 1,

    /// <summary>Not equal comparison.</summary>
    NotEquals = 2,

    /// <summary>String contains (case-insensitive by default).</summary>
    Contains = 3,

    /// <summary>String starts with.</summary>
    StartsWith = 4,

    /// <summary>String ends with.</summary>
    EndsWith = 5,

    /// <summary>Greater than comparison.</summary>
    GreaterThan = 6,

    /// <summary>Greater than or equal comparison.</summary>
    GreaterThanOrEqual = 7,

    /// <summary>Less than comparison.</summary>
    LessThan = 8,

    /// <summary>Less than or equal comparison.</summary>
    LessThanOrEqual = 9,

    /// <summary>Value is in a list of values.</summary>
    In = 10,

    /// <summary>Value is null.</summary>
    IsNull = 11,

    /// <summary>Value is not null.</summary>
    IsNotNull = 12,

    /// <summary>Value is between two bounds (inclusive).</summary>
    Between = 13
}

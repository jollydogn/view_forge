namespace ViewForge.Enums;

/// <summary>
/// Defines the logical operators for combining multiple filter conditions.
/// </summary>
public enum LogicalOperator
{
    /// <summary>All conditions must be true.</summary>
    And = 1,

    /// <summary>At least one condition must be true.</summary>
    Or = 2
}

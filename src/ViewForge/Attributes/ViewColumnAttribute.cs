namespace ViewForge.Attributes;

/// <summary>
/// Maps a property to a specific column name in the SQL view.
/// Use this when the property name differs from the column name.
/// </summary>
/// <example>
/// <code>
/// [ViewColumn("total_qty")]
/// public int TotalQuantity { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ViewColumnAttribute : Attribute
{
    /// <summary>
    /// The column name in the SQL view.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Creates a new ViewColumnAttribute with the specified column name.
    /// </summary>
    /// <param name="columnName">The column name in the SQL view.</param>
    public ViewColumnAttribute(string columnName)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
    }
}

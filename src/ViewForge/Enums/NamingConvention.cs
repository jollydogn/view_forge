namespace ViewForge.Enums;

/// <summary>
/// Defines naming conventions for mapping between C# property names and database column names.
/// </summary>
public enum NamingConvention
{
    /// <summary>No conversion — use property names as-is.</summary>
    None = 1,

    /// <summary>Convert to snake_case (e.g., ProductName → product_name).</summary>
    SnakeCase = 2,

    /// <summary>Convert to camelCase (e.g., ProductName → productName).</summary>
    CamelCase = 3,

    /// <summary>Use PascalCase (default C# convention).</summary>
    PascalCase = 4
}

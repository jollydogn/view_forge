using ViewForge.Enums;

namespace ViewForge.Mapping;

/// <summary>
/// Provides naming convention conversion utilities for mapping
/// between C# property names and SQL column names.
/// </summary>
public static class NamingConventionHelper
{
    /// <summary>
    /// Converts a property name to the specified naming convention.
    /// </summary>
    /// <param name="propertyName">The original property name (PascalCase).</param>
    /// <param name="convention">The target naming convention.</param>
    /// <returns>The converted name.</returns>
    public static string Convert(string propertyName, NamingConvention convention)
    {
        return convention switch
        {
            NamingConvention.SnakeCase => ToSnakeCase(propertyName),
            NamingConvention.CamelCase => ToCamelCase(propertyName),
            NamingConvention.PascalCase => propertyName,
            NamingConvention.None => propertyName,
            _ => propertyName
        };
    }

    /// <summary>
    /// Converts PascalCase to snake_case (e.g., ProductName → product_name).
    /// </summary>
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts PascalCase to camelCase (e.g., ProductName → productName).
    /// </summary>
    public static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    /// <summary>
    /// Checks if two names match regardless of naming convention
    /// (e.g., "ProductName" matches "product_name" or "productName").
    /// </summary>
    public static bool NamesMatch(string name1, string name2)
    {
        if (string.Equals(name1, name2, StringComparison.OrdinalIgnoreCase))
            return true;

        var snake1 = ToSnakeCase(name1);
        var snake2 = ToSnakeCase(name2);

        return string.Equals(snake1, snake2, StringComparison.OrdinalIgnoreCase);
    }
}

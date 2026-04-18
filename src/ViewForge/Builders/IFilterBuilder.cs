using System.Linq.Expressions;
using ViewForge.Models;

namespace ViewForge.Builders;

/// <summary>
/// Defines the contract for building filter expressions from filter descriptors.
/// </summary>
public interface IFilterBuilder
{
    /// <summary>
    /// Builds a LINQ expression from a filter group.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="filterGroup">The filter group containing conditions.</param>
    /// <returns>A compiled LINQ expression, or null if no valid filters exist.</returns>
    Expression<Func<T, bool>>? Build<T>(FilterGroup? filterGroup) where T : class;

    /// <summary>
    /// Builds a LINQ expression from a single filter descriptor.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="filter">The filter descriptor.</param>
    /// <returns>A compiled LINQ expression.</returns>
    Expression<Func<T, bool>> Build<T>(FilterDescriptor filter) where T : class;
}

using System.Linq.Expressions;
using ViewForge.Models;

namespace ViewForge.Builders;

/// <summary>
/// Defines the contract for building sort expressions from sort descriptors.
/// </summary>
public interface ISortBuilder
{
    /// <summary>
    /// Applies sorting to an IQueryable using the specified sort descriptors.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="query">The source queryable.</param>
    /// <param name="sortDescriptors">The sort descriptors to apply.</param>
    /// <returns>An ordered queryable.</returns>
    IQueryable<T> ApplySort<T>(IQueryable<T> query, IEnumerable<SortDescriptor> sortDescriptors) where T : class;
}

using Microsoft.Extensions.DependencyInjection;
using ViewForge.Builders;
using ViewForge.Configuration;
using ViewForge.Mapping;

namespace ViewForge.Extensions;

/// <summary>
/// Extension methods for integrating ViewForge into the .NET dependency injection container.
/// </summary>
public static class ViewForgeExtensions
{
    /// <summary>
    /// Registers ViewForge services including the filter builder, sort builder,
    /// view mapper, filter parser, and configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional action to configure <see cref="ViewForgeOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddViewForge(options =>
    /// {
    ///     options.DefaultNamingConvention = NamingConvention.SnakeCase;
    ///     options.DefaultPageSize = 25;
    ///     options.CaseInsensitiveFilters = true;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddViewForge(
        this IServiceCollection services,
        Action<ViewForgeOptions>? configure = null)
    {
        var options = new ViewForgeOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IViewMapper>(new ViewMapper(options));
        services.AddSingleton<IFilterBuilder>(sp =>
            new FilterBuilder(sp.GetRequiredService<IViewMapper>(), options));
        services.AddSingleton<ISortBuilder>(sp =>
            new SortBuilder(sp.GetRequiredService<IViewMapper>(), options));
        services.AddSingleton(new FilterParser(options));

        return services;
    }
}

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using ViewForge.Configuration;
using ViewForge.Enums;
using ViewForge.Mapping;
using ViewForge.Models;

namespace ViewForge.Builders;

/// <summary>
/// Builds LINQ expression trees from filter descriptors.
/// Supports all primitive types, DateTime, Guid, and their nullable variants.
/// This is the core engine that powers ViewForge's dynamic filtering.
/// </summary>
public class FilterBuilder : IFilterBuilder
{
    private readonly IViewMapper _mapper;
    private readonly ViewForgeOptions _options;

    /// <summary>
    /// Creates a new FilterBuilder with the specified dependencies.
    /// </summary>
    public FilterBuilder(IViewMapper mapper, ViewForgeOptions options)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public Expression<Func<T, bool>>? Build<T>(FilterGroup? filterGroup) where T : class
    {
        if (filterGroup is null)
            return null;

        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = BuildGroupExpression<T>(filterGroup, parameter);

        if (expression is null)
            return null;

        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    /// <inheritdoc />
    public Expression<Func<T, bool>> Build<T>(FilterDescriptor filter) where T : class
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = BuildFilterExpression<T>(filter, parameter);
        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private Expression? BuildGroupExpression<T>(FilterGroup group, ParameterExpression parameter) where T : class
    {
        Expression? combinedExpression = null;

        // Process individual filters
        foreach (var filter in group.Filters)
        {
            var filterExpression = BuildFilterExpression<T>(filter, parameter);
            combinedExpression = CombineExpressions(combinedExpression, filterExpression, group.Logic);
        }

        // Process nested groups
        foreach (var nestedGroup in group.Groups)
        {
            var nestedExpression = BuildGroupExpression<T>(nestedGroup, parameter);
            if (nestedExpression is not null)
            {
                combinedExpression = CombineExpressions(combinedExpression, nestedExpression, group.Logic);
            }
        }

        return combinedExpression;
    }

    private Expression BuildFilterExpression<T>(FilterDescriptor filter, ParameterExpression parameter) where T : class
    {
        var property = _mapper.GetProperty<T>(filter.PropertyName);
        if (property is null)
        {
            throw new InvalidOperationException(
                $"Property '{filter.PropertyName}' not found on type '{typeof(T).Name}'. " +
                $"Ensure the property exists and is not marked with [ViewIgnore].");
        }

        var propertyAccess = Expression.Property(parameter, property);
        var propertyType = property.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        var isNullable = Nullable.GetUnderlyingType(propertyType) is not null;

        return filter.Operator switch
        {
            FilterOperator.Equals => BuildComparisonExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value, Expression.Equal),
            FilterOperator.NotEquals => BuildComparisonExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value, Expression.NotEqual),
            FilterOperator.GreaterThan => BuildComparisonExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value, Expression.GreaterThan),
            FilterOperator.GreaterThanOrEqual => BuildComparisonExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value, Expression.GreaterThanOrEqual),
            FilterOperator.LessThan => BuildComparisonExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value, Expression.LessThan),
            FilterOperator.LessThanOrEqual => BuildComparisonExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value, Expression.LessThanOrEqual),
            FilterOperator.Contains => BuildStringExpression(propertyAccess, propertyType, isNullable, filter.Value, nameof(string.Contains)),
            FilterOperator.StartsWith => BuildStringExpression(propertyAccess, propertyType, isNullable, filter.Value, nameof(string.StartsWith)),
            FilterOperator.EndsWith => BuildStringExpression(propertyAccess, propertyType, isNullable, filter.Value, nameof(string.EndsWith)),
            FilterOperator.IsNull => BuildNullCheckExpression(propertyAccess, propertyType, isNullable, isNull: true),
            FilterOperator.IsNotNull => BuildNullCheckExpression(propertyAccess, propertyType, isNullable, isNull: false),
            FilterOperator.In => BuildInExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value),
            FilterOperator.Between => BuildBetweenExpression(propertyAccess, propertyType, underlyingType, isNullable, filter.Value, filter.ValueTo),
            _ => throw new NotSupportedException($"Filter operator '{filter.Operator}' is not supported.")
        };
    }

    private Expression BuildComparisonExpression(
        MemberExpression propertyAccess,
        Type propertyType,
        Type underlyingType,
        bool isNullable,
        object? value,
        Func<Expression, Expression, Expression> comparisonFactory)
    {
        var convertedValue = ConvertValue(value, underlyingType);
        var constant = Expression.Constant(convertedValue, propertyType);

        if (isNullable)
        {
            // For nullable types: x.Property.HasValue && x.Property == value
            var hasValue = Expression.Property(propertyAccess, "HasValue");
            var comparison = comparisonFactory(propertyAccess, constant);
            return Expression.AndAlso(hasValue, comparison);
        }

        return comparisonFactory(propertyAccess, constant);
    }

    private Expression BuildStringExpression(
        MemberExpression propertyAccess,
        Type propertyType,
        bool isNullable,
        object? value,
        string methodName)
    {
        if (propertyType != typeof(string) && (Nullable.GetUnderlyingType(propertyType) ?? propertyType) != typeof(string))
        {
            throw new InvalidOperationException(
                $"String filter operator '{methodName}' can only be applied to string properties.");
        }

        var stringValue = value?.ToString() ?? string.Empty;

        if (_options.CaseInsensitiveFilters)
        {
            stringValue = stringValue.ToLowerInvariant();
        }

        var constant = Expression.Constant(stringValue, typeof(string));

        Expression target = propertyAccess;

        // Null check for string: x.Property != null
        var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));

        if (_options.CaseInsensitiveFilters)
        {
            // x.Property.ToLower()
            var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
            target = Expression.Call(target, toLowerMethod);
        }

        // x.Property.Contains/StartsWith/EndsWith(value)
        var method = typeof(string).GetMethod(methodName, new[] { typeof(string) })!;
        var methodCall = Expression.Call(target, method, constant);

        return Expression.AndAlso(nullCheck, methodCall);
    }

    private static Expression BuildNullCheckExpression(
        MemberExpression propertyAccess,
        Type propertyType,
        bool isNullable,
        bool isNull)
    {
        if (isNullable)
        {
            var hasValue = Expression.Property(propertyAccess, "HasValue");
            return isNull ? Expression.Not(hasValue) : hasValue;
        }

        if (propertyType == typeof(string))
        {
            var nullConstant = Expression.Constant(null, typeof(string));
            return isNull
                ? Expression.Equal(propertyAccess, nullConstant)
                : Expression.NotEqual(propertyAccess, nullConstant);
        }

        // Non-nullable value types: IsNull is always false, IsNotNull is always true
        return Expression.Constant(!isNull);
    }

    private Expression BuildInExpression(
        MemberExpression propertyAccess,
        Type propertyType,
        Type underlyingType,
        bool isNullable,
        object? value)
    {
        if (value is null)
            return Expression.Constant(false);

        // Parse value as a comma-separated list
        var values = ParseInValues(value, underlyingType);
        var listType = typeof(List<>).MakeGenericType(underlyingType);
        var list = Activator.CreateInstance(listType)!;
        var addMethod = listType.GetMethod("Add")!;

        foreach (var v in values)
        {
            addMethod.Invoke(list, new[] { v });
        }

        var listConstant = Expression.Constant(list, listType);
        var containsMethod = listType.GetMethod("Contains", new[] { underlyingType })!;

        Expression propertyForContains = propertyAccess;
        if (isNullable)
        {
            // Use .Value for nullable types
            propertyForContains = Expression.Property(propertyAccess, "Value");
            var hasValue = Expression.Property(propertyAccess, "HasValue");
            var containsCall = Expression.Call(listConstant, containsMethod, propertyForContains);
            return Expression.AndAlso(hasValue, containsCall);
        }

        return Expression.Call(listConstant, containsMethod, propertyForContains);
    }

    private Expression BuildBetweenExpression(
        MemberExpression propertyAccess,
        Type propertyType,
        Type underlyingType,
        bool isNullable,
        object? valueFrom,
        object? valueTo)
    {
        var from = ConvertValue(valueFrom, underlyingType);
        var to = ConvertValue(valueTo, underlyingType);

        var fromConstant = Expression.Constant(from, propertyType);
        var toConstant = Expression.Constant(to, propertyType);

        if (isNullable)
        {
            var hasValue = Expression.Property(propertyAccess, "HasValue");
            var gte = Expression.GreaterThanOrEqual(propertyAccess, fromConstant);
            var lte = Expression.LessThanOrEqual(propertyAccess, toConstant);
            var between = Expression.AndAlso(gte, lte);
            return Expression.AndAlso(hasValue, between);
        }

        var gteExpr = Expression.GreaterThanOrEqual(propertyAccess, fromConstant);
        var lteExpr = Expression.LessThanOrEqual(propertyAccess, toConstant);
        return Expression.AndAlso(gteExpr, lteExpr);
    }

    private static Expression CombineExpressions(Expression? existing, Expression newExpression, LogicalOperator logic)
    {
        if (existing is null)
            return newExpression;

        return logic switch
        {
            LogicalOperator.And => Expression.AndAlso(existing, newExpression),
            LogicalOperator.Or => Expression.OrElse(existing, newExpression),
            _ => Expression.AndAlso(existing, newExpression)
        };
    }

    /// <summary>
    /// Converts a raw value to the specified target type.
    /// Supports string, int, long, double, decimal, float, bool, DateTime, DateTimeOffset, Guid, TimeSpan.
    /// </summary>
    internal static object? ConvertValue(object? value, Type targetType)
    {
        if (value is null)
            return null;

        if (targetType.IsInstanceOfType(value))
            return value;

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
            return null;

        if (targetType == typeof(string))
            return stringValue;

        if (targetType == typeof(Guid))
            return Guid.Parse(stringValue);

        if (targetType == typeof(DateTime))
            return DateTime.Parse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        if (targetType == typeof(DateTimeOffset))
            return DateTimeOffset.Parse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        if (targetType == typeof(TimeSpan))
            return TimeSpan.Parse(stringValue, CultureInfo.InvariantCulture);

        if (targetType == typeof(bool))
            return bool.Parse(stringValue);

        if (targetType.IsEnum)
            return Enum.Parse(targetType, stringValue, ignoreCase: true);

        return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }

    private static List<object> ParseInValues(object value, Type targetType)
    {
        var result = new List<object>();
        var stringValue = value.ToString();

        if (string.IsNullOrEmpty(stringValue))
            return result;

        var parts = stringValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            var converted = ConvertValue(part, targetType);
            if (converted is not null)
                result.Add(converted);
        }

        return result;
    }
}

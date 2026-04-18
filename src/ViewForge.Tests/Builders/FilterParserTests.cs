using ViewForge.Builders;
using ViewForge.Configuration;
using ViewForge.Enums;
using Xunit;

namespace ViewForge.Tests.Builders;

public class FilterParserTests
{
    private readonly FilterParser _sut;

    public FilterParserTests()
    {
        _sut = new FilterParser(new ViewForgeOptions());
    }

    [Fact]
    public void Parse_SimpleFilter_ReturnsCorrectDescriptor()
    {
        var result = _sut.Parse("Name~contains~keyboard");

        Assert.NotNull(result);
        Assert.Single(result!.Filters);
        Assert.Equal("Name", result.Filters[0].PropertyName);
        Assert.Equal(FilterOperator.Contains, result.Filters[0].Operator);
        Assert.Equal("keyboard", result.Filters[0].Value);
    }

    [Fact]
    public void Parse_MultipleFilters_ReturnsAll()
    {
        var result = _sut.Parse("Name~contains~test;Price~gt~100;IsActive~eq~true");

        Assert.NotNull(result);
        Assert.Equal(3, result!.Filters.Count);
        Assert.Equal(LogicalOperator.And, result.Logic);
    }

    [Fact]
    public void Parse_IsNull_NoValueRequired()
    {
        var result = _sut.Parse("SupplierId~isnull");

        Assert.NotNull(result);
        Assert.Single(result!.Filters);
        Assert.Equal(FilterOperator.IsNull, result.Filters[0].Operator);
        Assert.Null(result.Filters[0].Value);
    }

    [Fact]
    public void Parse_Between_ParsesFromAndTo()
    {
        var result = _sut.Parse("CreatedDate~between~2024-01-01,2024-12-31");

        Assert.NotNull(result);
        Assert.Single(result!.Filters);
        Assert.Equal(FilterOperator.Between, result.Filters[0].Operator);
        Assert.Equal("2024-01-01", result.Filters[0].Value);
        Assert.Equal("2024-12-31", result.Filters[0].ValueTo);
    }

    [Fact]
    public void Parse_AllOperatorAliases_Recognized()
    {
        var aliases = new Dictionary<string, FilterOperator>
        {
            ["eq"] = FilterOperator.Equals,
            ["neq"] = FilterOperator.NotEquals,
            ["contains"] = FilterOperator.Contains,
            ["startswith"] = FilterOperator.StartsWith,
            ["endswith"] = FilterOperator.EndsWith,
            ["gt"] = FilterOperator.GreaterThan,
            ["gte"] = FilterOperator.GreaterThanOrEqual,
            ["lt"] = FilterOperator.LessThan,
            ["lte"] = FilterOperator.LessThanOrEqual,
            ["in"] = FilterOperator.In,
            ["null"] = FilterOperator.IsNull,
            ["notnull"] = FilterOperator.IsNotNull,
            ["btw"] = FilterOperator.Between
        };

        foreach (var (alias, expected) in aliases)
        {
            var filterStr = alias is "null" or "notnull"
                ? $"Name~{alias}"
                : $"Name~{alias}~value";

            var result = _sut.Parse(filterStr);
            Assert.NotNull(result);
            Assert.Equal(expected, result!.Filters[0].Operator);
        }
    }

    [Fact]
    public void Parse_EmptyString_ReturnsNull()
    {
        Assert.Null(_sut.Parse(null));
        Assert.Null(_sut.Parse(""));
        Assert.Null(_sut.Parse("   "));
    }

    [Fact]
    public void Parse_InvalidOperator_SkipsFilter()
    {
        var result = _sut.Parse("Name~invalidop~value");
        Assert.Null(result);
    }
}

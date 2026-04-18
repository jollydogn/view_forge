using ViewForge.Builders;
using ViewForge.Configuration;
using ViewForge.Enums;
using ViewForge.Mapping;
using ViewForge.Models;
using ViewForge.Tests.TestModels;
using Xunit;

namespace ViewForge.Tests.Builders;

public class FilterBuilderTests
{
    private readonly FilterBuilder _sut;
    private readonly List<TestEntity> _data;

    public FilterBuilderTests()
    {
        var options = new ViewForgeOptions
        {
            DefaultNamingConvention = NamingConvention.PascalCase,
            CaseInsensitiveFilters = true
        };
        var mapper = new ViewMapper(options);
        _sut = new FilterBuilder(mapper, options);

        _data = new List<TestEntity>
        {
            new() { Id = Guid.NewGuid(), Name = "Alpha", Quantity = 10, Price = 99.99m, IsActive = true, CreatedDate = new DateTime(2024, 1, 1), CategoryId = Guid.NewGuid(), NullableCategoryId = Guid.NewGuid(), NullableDate = new DateTime(2024, 6, 1) },
            new() { Id = Guid.NewGuid(), Name = "Beta", Quantity = 20, Price = 149.99m, IsActive = false, CreatedDate = new DateTime(2024, 3, 15), CategoryId = Guid.NewGuid(), NullableCategoryId = null, NullableDate = null },
            new() { Id = Guid.NewGuid(), Name = "Gamma", Quantity = 5, Price = 29.99m, IsActive = true, CreatedDate = new DateTime(2024, 6, 20), CategoryId = Guid.NewGuid(), NullableCategoryId = Guid.NewGuid(), NullableDate = new DateTime(2024, 12, 25) },
            new() { Id = Guid.NewGuid(), Name = "Delta keyboard", Quantity = 0, Price = 0m, IsActive = false, CreatedDate = new DateTime(2023, 11, 1), CategoryId = Guid.NewGuid(), NullableCategoryId = null, NullableDate = null },
            new() { Id = Guid.NewGuid(), Name = "Epsilon", Description = "Special item", Quantity = 100, Price = 999.99m, IsActive = true, CreatedDate = new DateTime(2024, 9, 30), CategoryId = Guid.NewGuid(), NullableCategoryId = Guid.NewGuid(), NullableDate = new DateTime(2025, 1, 1) },
        };
    }

    [Fact]
    public void Equals_String_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("Name", FilterOperator.Equals, "Alpha");
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Single(result);
        Assert.Equal("Alpha", result[0].Name);
    }

    [Fact]
    public void Contains_String_CaseInsensitive()
    {
        var filter = FilterDescriptor.Create("Name", FilterOperator.Contains, "KEYBOARD");
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Single(result);
        Assert.Contains("keyboard", result[0].Name);
    }

    [Fact]
    public void GreaterThan_Int_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("Quantity", FilterOperator.GreaterThan, 10);
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.True(e.Quantity > 10));
    }

    [Fact]
    public void LessThanOrEqual_Decimal_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("Price", FilterOperator.LessThanOrEqual, 99.99m);
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, e => Assert.True(e.Price <= 99.99m));
    }

    [Fact]
    public void Equals_Bool_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("IsActive", FilterOperator.Equals, true);
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, e => Assert.True(e.IsActive));
    }

    [Fact]
    public void IsNull_NullableGuid_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("NullableCategoryId", FilterOperator.IsNull);
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Null(e.NullableCategoryId));
    }

    [Fact]
    public void IsNotNull_NullableDateTime_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("NullableDate", FilterOperator.IsNotNull);
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, e => Assert.NotNull(e.NullableDate));
    }

    [Fact]
    public void Between_DateTime_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("CreatedDate", FilterOperator.Between,
            new DateTime(2024, 1, 1), new DateTime(2024, 6, 30));
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void In_String_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("Name", FilterOperator.In, "Alpha,Gamma,Epsilon");
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void StartsWith_String_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("Name", FilterOperator.StartsWith, "al");
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Single(result);
        Assert.Equal("Alpha", result[0].Name);
    }

    [Fact]
    public void FilterGroup_And_CombinesCorrectly()
    {
        var group = FilterGroup.And(
            FilterDescriptor.Create("IsActive", FilterOperator.Equals, true),
            FilterDescriptor.Create("Price", FilterOperator.GreaterThan, 50m)
        );

        var expr = _sut.Build<TestEntity>(group);
        Assert.NotNull(expr);

        var result = _data.AsQueryable().Where(expr!).ToList();
        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.True(e.IsActive && e.Price > 50m));
    }

    [Fact]
    public void FilterGroup_Or_CombinesCorrectly()
    {
        var group = FilterGroup.Or(
            FilterDescriptor.Create("Quantity", FilterOperator.Equals, 0),
            FilterDescriptor.Create("Quantity", FilterOperator.Equals, 100)
        );

        var expr = _sut.Build<TestEntity>(group);
        Assert.NotNull(expr);

        var result = _data.AsQueryable().Where(expr!).ToList();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void NullFilterGroup_ReturnsNull()
    {
        var expr = _sut.Build<TestEntity>((FilterGroup?)null);
        Assert.Null(expr);
    }

    [Fact]
    public void InvalidProperty_ThrowsException()
    {
        var filter = FilterDescriptor.Create("NonExistentProperty", FilterOperator.Equals, "test");
        Assert.Throws<InvalidOperationException>(() => _sut.Build<TestEntity>(filter));
    }

    [Fact]
    public void IsNull_String_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("Description", FilterOperator.IsNull);
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(4, result.Count);
        Assert.All(result, e => Assert.Null(e.Description));
    }

    [Fact]
    public void NotEquals_Int_FiltersCorrectly()
    {
        var filter = FilterDescriptor.Create("Quantity", FilterOperator.NotEquals, 10);
        var expr = _sut.Build<TestEntity>(filter);
        var result = _data.AsQueryable().Where(expr).ToList();

        Assert.Equal(4, result.Count);
        Assert.All(result, e => Assert.NotEqual(10, e.Quantity));
    }
}

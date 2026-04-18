using ViewForge.Configuration;
using ViewForge.Enums;
using ViewForge.Mapping;
using ViewForge.Tests.TestModels;
using Xunit;

namespace ViewForge.Tests.Mapping;

public class ViewMapperTests
{
    private readonly ViewMapper _sut;

    public ViewMapperTests()
    {
        var options = new ViewForgeOptions { DefaultNamingConvention = NamingConvention.SnakeCase };
        _sut = new ViewMapper(options);
    }

    [Fact]
    public void GetViewName_WithAttribute_ReturnsAttributeName()
    {
        var name = _sut.GetViewName<TestEntity>();
        Assert.Equal("vw_test_entity", name);
    }

    [Fact]
    public void GetMappableProperties_ExcludesViewIgnored()
    {
        var props = _sut.GetMappableProperties<TestEntity>();

        Assert.DoesNotContain(props, p => p.Name == "Computed");
        Assert.Contains(props, p => p.Name == "Name");
        Assert.Contains(props, p => p.Name == "NullableCategoryId");
    }

    [Fact]
    public void GetColumnName_WithAttribute_ReturnsCustomColumn()
    {
        var prop = typeof(TestEntity).GetProperty("MappedColumn")!;
        var columnName = _sut.GetColumnName(prop);
        Assert.Equal("custom_col", columnName);
    }

    [Fact]
    public void GetColumnName_WithoutAttribute_UsesNamingConvention()
    {
        var prop = typeof(TestEntity).GetProperty("ProductName")
                   ?? typeof(TestEntity).GetProperty("Name")!;
        var columnName = _sut.GetColumnName(prop);
        Assert.Equal("name", columnName);
    }

    [Fact]
    public void GetProperty_ByPropertyName_FindsProperty()
    {
        var prop = _sut.GetProperty<TestEntity>("Name");
        Assert.NotNull(prop);
        Assert.Equal("Name", prop!.Name);
    }

    [Fact]
    public void GetProperty_ByColumnName_FindsProperty()
    {
        var prop = _sut.GetProperty<TestEntity>("custom_col");
        Assert.NotNull(prop);
        Assert.Equal("MappedColumn", prop!.Name);
    }

    [Fact]
    public void GetProperty_IgnoredProperty_ReturnsNull()
    {
        var prop = _sut.GetProperty<TestEntity>("Computed");
        Assert.Null(prop);
    }

    [Fact]
    public void GetProperty_NonExistent_ReturnsNull()
    {
        var prop = _sut.GetProperty<TestEntity>("FakeProperty");
        Assert.Null(prop);
    }
}

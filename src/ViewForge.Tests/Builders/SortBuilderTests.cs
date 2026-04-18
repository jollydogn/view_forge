using ViewForge.Builders;
using ViewForge.Configuration;
using ViewForge.Enums;
using ViewForge.Mapping;
using ViewForge.Models;
using ViewForge.Tests.TestModels;
using Xunit;

namespace ViewForge.Tests.Builders;

public class SortBuilderTests
{
    private readonly SortBuilder _sut;
    private readonly List<TestEntity> _data;

    public SortBuilderTests()
    {
        var options = new ViewForgeOptions { DefaultNamingConvention = NamingConvention.PascalCase };
        var mapper = new ViewMapper(options);
        _sut = new SortBuilder(mapper, options);

        _data = new List<TestEntity>
        {
            new() { Name = "Charlie", Quantity = 30, Price = 200m },
            new() { Name = "Alpha", Quantity = 10, Price = 100m },
            new() { Name = "Bravo", Quantity = 20, Price = 300m },
        };
    }

    [Fact]
    public void OrderBy_Ascending_SortsCorrectly()
    {
        var descriptors = new List<SortDescriptor>
        {
            SortDescriptor.Create("Name", SortDirection.Ascending)
        };

        var result = _sut.ApplySort(_data.AsQueryable(), descriptors).ToList();

        Assert.Equal("Alpha", result[0].Name);
        Assert.Equal("Bravo", result[1].Name);
        Assert.Equal("Charlie", result[2].Name);
    }

    [Fact]
    public void OrderBy_Descending_SortsCorrectly()
    {
        var descriptors = new List<SortDescriptor>
        {
            SortDescriptor.Create("Price", SortDirection.Descending)
        };

        var result = _sut.ApplySort(_data.AsQueryable(), descriptors).ToList();

        Assert.Equal(300m, result[0].Price);
        Assert.Equal(200m, result[1].Price);
        Assert.Equal(100m, result[2].Price);
    }

    [Fact]
    public void EmptyDescriptors_ReturnsOriginalOrder()
    {
        var result = _sut.ApplySort(_data.AsQueryable(), new List<SortDescriptor>()).ToList();
        Assert.Equal("Charlie", result[0].Name);
    }

    [Fact]
    public void InvalidProperty_ThrowsException()
    {
        var descriptors = new List<SortDescriptor>
        {
            SortDescriptor.Create("FakeProperty", SortDirection.Ascending)
        };

        Assert.Throws<InvalidOperationException>(() =>
            _sut.ApplySort(_data.AsQueryable(), descriptors).ToList());
    }

    [Fact]
    public void Parse_DescendingSyntax_Works()
    {
        var result = SortBuilder.Parse("Name desc, Price asc");

        Assert.Equal(2, result.Count);
        Assert.Equal("Name", result[0].PropertyName);
        Assert.Equal(SortDirection.Descending, result[0].Direction);
        Assert.Equal("Price", result[1].PropertyName);
        Assert.Equal(SortDirection.Ascending, result[1].Direction);
    }

    [Fact]
    public void Parse_MinusSyntax_Works()
    {
        var result = SortBuilder.Parse("-Price,Name");

        Assert.Equal(2, result.Count);
        Assert.Equal("Price", result[0].PropertyName);
        Assert.Equal(SortDirection.Descending, result[0].Direction);
        Assert.Equal("Name", result[1].PropertyName);
        Assert.Equal(SortDirection.Ascending, result[1].Direction);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsEmpty()
    {
        var result = SortBuilder.Parse(null);
        Assert.Empty(result);

        result = SortBuilder.Parse("");
        Assert.Empty(result);
    }
}

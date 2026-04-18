using ViewForge.Mapping;
using Xunit;

namespace ViewForge.Tests.Mapping;

public class NamingConventionTests
{
    [Theory]
    [InlineData("ProductName", "product_name")]
    [InlineData("Id", "id")]
    [InlineData("TotalSalesAmount", "total_sales_amount")]
    [InlineData("HTTPSUrl", "h_t_t_p_s_url")]
    [InlineData("", "")]
    public void ToSnakeCase_ConvertsCorrectly(string input, string expected)
    {
        Assert.Equal(expected, NamingConventionHelper.ToSnakeCase(input));
    }

    [Theory]
    [InlineData("ProductName", "productName")]
    [InlineData("Id", "id")]
    [InlineData("A", "a")]
    public void ToCamelCase_ConvertsCorrectly(string input, string expected)
    {
        Assert.Equal(expected, NamingConventionHelper.ToCamelCase(input));
    }

    [Theory]
    [InlineData("ProductName", "product_name", true)]
    [InlineData("ProductName", "ProductName", true)]
    [InlineData("ProductName", "productname", true)]
    [InlineData("ProductName", "SomethingElse", false)]
    public void NamesMatch_WorksCorrectly(string name1, string name2, bool expected)
    {
        Assert.Equal(expected, NamingConventionHelper.NamesMatch(name1, name2));
    }
}

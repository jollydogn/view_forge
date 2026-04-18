using ViewForge.Sample.Views;

namespace ViewForge.Sample.Data;

/// <summary>
/// Seeds sample data for demonstration purposes.
/// </summary>
public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.ProductSummaries.Any())
            return;

        var supplier1 = Guid.NewGuid();
        var supplier2 = Guid.NewGuid();

        context.ProductSummaries.AddRange(
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Mechanical Keyboard", CategoryName = "Electronics",
                Price = 149.99m, StockQuantity = 250, TotalSales = 37500m, IsActive = true,
                CreatedDate = new DateTime(2024, 1, 15), SupplierId = supplier1, SupplierName = "TechParts Inc."
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Wireless Mouse", CategoryName = "Electronics",
                Price = 49.99m, StockQuantity = 500, TotalSales = 24995m, IsActive = true,
                CreatedDate = new DateTime(2024, 2, 20), SupplierId = supplier1, SupplierName = "TechParts Inc."
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "USB-C Hub", CategoryName = "Accessories",
                Price = 79.99m, StockQuantity = 150, TotalSales = 11998m, IsActive = true,
                CreatedDate = new DateTime(2024, 3, 10), SupplierId = supplier2, SupplierName = "ConnectPlus"
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Monitor Stand", CategoryName = "Furniture",
                Price = 89.99m, StockQuantity = 75, TotalSales = 6749m, IsActive = true,
                CreatedDate = new DateTime(2024, 4, 5), SupplierId = null, SupplierName = null
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Webcam HD", CategoryName = "Electronics",
                Price = 129.99m, StockQuantity = 0, TotalSales = 15599m, IsActive = false,
                CreatedDate = new DateTime(2023, 11, 1), SupplierId = supplier1, SupplierName = "TechParts Inc."
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Desk Lamp", CategoryName = "Furniture",
                Price = 39.99m, StockQuantity = 300, TotalSales = 11997m, IsActive = true,
                CreatedDate = new DateTime(2024, 5, 22), SupplierId = supplier2, SupplierName = "ConnectPlus"
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Laptop Sleeve", CategoryName = "Accessories",
                Price = 29.99m, StockQuantity = 1000, TotalSales = 29990m, IsActive = true,
                CreatedDate = new DateTime(2024, 6, 1), SupplierId = null, SupplierName = null
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Noise Cancelling Headphones", CategoryName = "Electronics",
                Price = 299.99m, StockQuantity = 80, TotalSales = 23999m, IsActive = true,
                CreatedDate = new DateTime(2024, 7, 15), SupplierId = supplier1, SupplierName = "TechParts Inc."
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Ergonomic Chair", CategoryName = "Furniture",
                Price = 549.99m, StockQuantity = 20, TotalSales = 10999m, IsActive = true,
                CreatedDate = new DateTime(2024, 8, 30), SupplierId = supplier2, SupplierName = "ConnectPlus"
            },
            new ProductSummaryView
            {
                Id = Guid.NewGuid(), ProductName = "Screen Protector Pack", CategoryName = "Accessories",
                Price = 9.99m, StockQuantity = 5000, TotalSales = 49950m, IsActive = true,
                CreatedDate = new DateTime(2024, 9, 12), SupplierId = null, SupplierName = null
            }
        );

        context.SaveChanges();
    }
}

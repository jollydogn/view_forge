# ViewForge ⚒️

[![.NET](https://img.shields.io/badge/.NET-7.0%20|%208.0%20|%209.0%20|%2010.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-ViewForge-blue?style=flat-square&logo=nuget)](https://www.nuget.org/packages/ViewForge)
[![Build](https://github.com/jollydogn/view_forge/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/jollydogn/view_forge/actions)

A high-performance .NET library that dynamically maps SQL Views to strongly-typed models with built-in **dynamic filtering**, **sorting**, and **pagination** — all via a clean, fluent API. No manual SQL building required.

---

## ✨ Features

- 🎯 **Auto Model Mapping** — Maps view columns to C# properties (snake_case, PascalCase, camelCase, custom)
- 🔍 **Dynamic Filtering** — Filter by any field: `string`, `int`, `double`, `decimal`, `DateTime`, `Guid`, `bool`, and **all nullable variants**
- 📊 **Dynamic Sorting** — Sort by any property with `asc`/`desc` support, multi-column sorting
- 📄 **Pagination** — Built-in `PagedResult<T>` with fully parameterized `page`/`pageSize`
- 🏷️ **Attribute-Based Config** — `[ViewName]`, `[ViewColumn]`, `[ViewIgnore]`, `[Filterable]`, `[Sortable]`
- ⚡ **Expression Tree Based** — Builds `Expression<Func<T, bool>>` dynamically — SQL-injection safe
- 🎛️ **Fluent API + String API** — Use type-safe expressions or query string filters
- 🌍 **Multi-Target** — .NET 7 / .NET 8 / .NET 9 / .NET 10

---

## 📦 Installation

```bash
dotnet add package ViewForge
```

Or via Package Manager:

```powershell
Install-Package ViewForge
```

---

## 🚀 Quick Start

### 1. Define Your View Model

```csharp
using ViewForge.Attributes;

[ViewName("vw_product_summary")]
public class ProductSummaryView
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public decimal TotalSales { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    [ViewIgnore]
    public string DisplayName => $"{ProductName} ({CategoryName})";
}
```

### 2. Register the View in DbContext

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Tell EF Core this is a SQL View, not a table
    modelBuilder.Entity<ProductSummaryView>(entity =>
    {
        entity.HasNoKey();
        entity.ToView("vw_product_summary", "report"); // schema + view name
    });
}
```

### 3. Register ViewForge & Layers

```csharp
// Register ViewForge
builder.Services.AddViewForge(options =>
{
    options.DefaultNamingConvention = NamingConvention.SnakeCase;
    options.DefaultPageSize = 25;
    options.MaxPageSize = 100;
    options.CaseInsensitiveFilters = true;
});

// Register application layers (Dependency Inversion)
builder.Services.AddScoped<IViewForgeSimpleRepository, ViewForgeSimpleRepository>();
builder.Services.AddScoped<IViewForgeSimpleAppService, ViewForgeSimpleAppService>();
```

### 4. Repository — All DB Logic Here

```csharp
public class ViewForgeSimpleRepository : IViewForgeSimpleRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IFilterBuilder _filterBuilder;
    private readonly ISortBuilder _sortBuilder;

    public async Task<PagedResult<ProductSummaryView>> GetFilteredListAsync(
        FilterGroup? filterGroup, List<SortDescriptor> sortDescriptors,
        int page, int pageSize, CancellationToken ct = default)
    {
        return await _dbContext.ProductSummaries.AsNoTracking()
            .ApplyFilters(filterGroup, _filterBuilder)
            .ApplySorting(sortDescriptors, _sortBuilder)
            .ToPagedResultAsync(page, pageSize, ct);
    }
}
```

### 5. AppService — Orchestration Only

```csharp
public class ViewForgeSimpleAppService : IViewForgeSimpleAppService
{
    private readonly IViewForgeSimpleRepository _repository;
    private readonly FilterParser _filterParser;

    public async Task<PagedResult<ProductSummaryView>> GetFilteredListAsync(
        string? filters, string? sorting,
        int page, int pageSize, CancellationToken ct = default)
    {
        var filterGroup = _filterParser.Parse(filters);
        var sortDescriptors = SortBuilder.Parse(sorting);
        return await _repository.GetFilteredListAsync(filterGroup, sortDescriptors, page, pageSize, ct);
    }
}
```

### 6. Controller — HTTP I/O Only

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IViewForgeSimpleAppService _appService;

    [HttpGet]
    public async Task<PagedResult<ProductSummaryView>> GetListAsync(
        [FromQuery] string? filters, [FromQuery] string? sorting,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        return await _appService.GetFilteredListAsync(filters, sorting, page, pageSize, ct);
    }
}
```

---

## 🔍 Dynamic Filtering

### Supported Filter Operators

| Operator | Alias | Example | Description |
|----------|-------|---------|-------------|
| Equals | `eq` | `Name~eq~Keyboard` | Exact match |
| NotEquals | `neq` | `Status~neq~Inactive` | Not equal |
| Contains | `contains` | `Name~contains~key` | String contains (case-insensitive) |
| StartsWith | `startswith` | `Name~startswith~Pro` | String starts with |
| EndsWith | `endswith` | `Name~endswith~board` | String ends with |
| GreaterThan | `gt` | `Price~gt~100` | Greater than |
| GreaterThanOrEqual | `gte` | `Price~gte~100` | Greater than or equal |
| LessThan | `lt` | `Quantity~lt~10` | Less than |
| LessThanOrEqual | `lte` | `Quantity~lte~10` | Less than or equal |
| In | `in` | `Category~in~A,B,C` | Value in list |
| IsNull | `null` | `SupplierId~null` | Value is null |
| IsNotNull | `notnull` | `SupplierId~notnull` | Value is not null |
| Between | `btw` | `Date~btw~2024-01-01,2024-12-31` | Between two values |

### Supported Types

All operators work with: `string`, `int`, `long`, `double`, `decimal`, `float`, `bool`, `DateTime`, `DateTimeOffset`, `Guid`, `TimeSpan` — and **all nullable variants** (`int?`, `DateTime?`, `Guid?`, etc.).

### Query String Format

```
GET /api/products?filters=ProductName~contains~keyboard;Price~gt~100;CreatedDate~btw~2024-01-01,2024-12-31&sorting=TotalSales desc,ProductName asc&page=1&pageSize=20
```

Multiple filters are separated by `;` and combined with AND logic.

### Programmatic API

```csharp
// Build filters programmatically
var filters = FilterGroup.And(
    FilterDescriptor.Create("ProductName", FilterOperator.Contains, "keyboard"),
    FilterDescriptor.Create("Price", FilterOperator.GreaterThan, 100m),
    FilterDescriptor.Create("CategoryId", FilterOperator.IsNotNull)
);

// Or use OR logic
var orFilters = FilterGroup.Or(
    FilterDescriptor.Create("Status", FilterOperator.Equals, "Active"),
    FilterDescriptor.Create("Priority", FilterOperator.Equals, "High")
);
```

---

## 📊 Dynamic Sorting

```csharp
// From query string
var sorts = SortBuilder.Parse("TotalSales desc, ProductName asc");

// Alternative minus syntax
var sorts2 = SortBuilder.Parse("-TotalSales, ProductName");

// Programmatic
var sorts3 = new List<SortDescriptor>
{
    SortDescriptor.Create("TotalSales", SortDirection.Descending),
    SortDescriptor.Create("ProductName", SortDirection.Ascending)
};
```

---

## 📄 Pagination

Pagination is **fully parameterized** — `page` and `pageSize` come from external sources (controller parameters, query strings, etc.):

```csharp
// In the Repository layer — page & pageSize flow from Controller → AppService → Repository
public async Task<PagedResult<ProductView>> GetFilteredListAsync(
    FilterGroup? filterGroup, List<SortDescriptor> sortDescriptors,
    int page, int pageSize, CancellationToken ct = default)
{
    return await _dbContext.ProductSummaries.AsNoTracking()
        .ApplyFilters(filterGroup, _filterBuilder)
        .ApplySorting(sortDescriptors, _sortBuilder)
        .ToPagedResultAsync(page, pageSize, ct);
}
```

### PagedResult Response

```json
{
    "items": [...],
    "totalCount": 150,
    "page": 1,
    "pageSize": 20,
    "totalPages": 8,
    "hasPreviousPage": false,
    "hasNextPage": true
}
```

---

## 🏷️ Attributes

| Attribute | Target | Description |
|-----------|--------|-------------|
| `[ViewName("name")]` | Class | Maps class to SQL view name |
| `[ViewColumn("col")]` | Property | Maps property to specific column name |
| `[ViewIgnore]` | Property | Excludes property from mapping/filtering |
| `[Filterable]` | Property | Marks property as filterable (with optional operator restriction) |
| `[Sortable]` | Property | Marks property as sortable |

```csharp
[ViewName("vw_orders", Schema = "sales")]
public class OrderView
{
    [ViewColumn("order_total")]
    public decimal Total { get; set; }

    [Filterable(AllowedOperators = new[] { FilterOperator.Equals, FilterOperator.In })]
    public string Status { get; set; }

    [ViewIgnore]
    public string ComputedField => "...";
}
```

---

## 🔧 Configuration

```csharp
builder.Services.AddViewForge(options =>
{
    options.DefaultNamingConvention = NamingConvention.SnakeCase;  // snake_case column mapping
    options.AllPropertiesFilterableByDefault = true;               // all props filterable
    options.AllPropertiesSortableByDefault = true;                 // all props sortable
    options.CaseInsensitiveFilters = true;                        // case-insensitive string filters
    options.DefaultPageSize = 20;                                  // default page size
    options.MaxPageSize = 100;                                     // max allowed page size
    options.FilterSeparator = "~";                                 // PropertyName~operator~value
    options.FilterDelimiter = ";";                                 // filter1;filter2;filter3
});
```

### Available Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `DefaultNamingConvention` | `NamingConvention` | `SnakeCase` | Column name mapping convention |
| `AllPropertiesFilterableByDefault` | `bool` | `true` | All properties filterable unless `[ViewIgnore]` |
| `AllPropertiesSortableByDefault` | `bool` | `true` | All properties sortable unless `[ViewIgnore]` |
| `CaseInsensitiveFilters` | `bool` | `true` | Case-insensitive string comparisons |
| `DefaultPageSize` | `int` | `20` | Default items per page |
| `MaxPageSize` | `int` | `100` | Maximum allowed page size |
| `FilterSeparator` | `string` | `~` | Separator within a filter expression |
| `FilterDelimiter` | `string` | `;` | Delimiter between multiple filters |

---

## 📁 Project Structure

```
ViewForge/
├── src/
│   ├── ViewForge/                     # Core NuGet library
│   │   ├── Attributes/               # ViewName, ViewColumn, ViewIgnore, Filterable, Sortable
│   │   ├── Builders/                  # FilterBuilder, SortBuilder, FilterParser
│   │   ├── Configuration/            # ViewForgeOptions
│   │   ├── Enums/                     # FilterOperator, SortDirection, LogicalOperator
│   │   ├── Extensions/               # DI registration, IQueryable extensions
│   │   ├── Mapping/                  # ViewMapper, NamingConventionHelper
│   │   ├── Models/                   # PagedResult, FilterDescriptor, SortDescriptor
│   │   └── Providers/                # IViewQueryProvider, EfCoreViewQueryProvider
│   │
│   ├── ViewForge.Sample/             # Sample API (SOLID architecture)
│   │   ├── Controllers/              # HTTP I/O only
│   │   ├── Services/                 # AppService layer (orchestration)
│   │   ├── Repositories/            # Repository layer (DB operations)
│   │   ├── Views/                    # View models
│   │   └── Data/                     # DbContext, Seed data
│   │
│   └── ViewForge.Tests/              # Unit tests (50 tests)
│
├── .github/workflows/                # CI/CD pipelines
├── ViewForge.sln
└── README.md
```

---

## 🤝 Contributing

Contributions are welcome! Feel free to open issues and submit pull requests.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🔗 Links

- [GitHub Repository](https://github.com/jollydogn/view_forge)
- [Report an Issue](https://github.com/jollydogn/view_forge/issues)

---

Made with ⚒️ by [ViewForge](https://github.com/jollydogn)

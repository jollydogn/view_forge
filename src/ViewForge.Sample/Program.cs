using Microsoft.EntityFrameworkCore;
using ViewForge.Enums;
using ViewForge.Extensions;
using ViewForge.Sample.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ViewForge Sample API",
        Version = "v1",
        Description = "Demonstrates ViewForge dynamic filtering, sorting, and pagination."
    });
});

// Register EF Core with InMemory provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ViewForgeSampleDb"));

// Register ViewForge
builder.Services.AddViewForge(options =>
{
    options.DefaultNamingConvention = NamingConvention.PascalCase;
    options.DefaultPageSize = 10;
    options.MaxPageSize = 50;
    options.CaseInsensitiveFilters = true;
});

var app = builder.Build();

// Seed sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(context);
}

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

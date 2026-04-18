using Microsoft.EntityFrameworkCore;
using ViewForge.Sample.Views;

namespace ViewForge.Sample.Data;

/// <summary>
/// Sample DbContext demonstrating how to register view models with EF Core.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Product summary view — in a real app, this maps to a SQL view via HasNoKey().ToView().
    /// For demo purposes, we use it as a regular DbSet with in-memory data.
    /// </summary>
    public DbSet<ProductSummaryView> ProductSummaries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductSummaryView>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Ignore(e => e.DisplayName);
        });
    }
}

using CheckoutService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Infrastructure.Data;

/// <summary>
///     EF Core DbContext for CheckoutService.
/// </summary>
public class CheckoutDbContext : DbContext
{
    public CheckoutDbContext(DbContextOptions<CheckoutDbContext> options)
        : base(options)
    {
    }

    public DbSet<Checkout> Checkouts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API configurations for the Checkout entity
        modelBuilder.Entity<Checkout>().HasKey(c => c.Id);
    }
}
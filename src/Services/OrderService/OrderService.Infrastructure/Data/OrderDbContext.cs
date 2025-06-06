using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Models;

namespace OrderService.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ShippingInfo> ShippingInfos => Set<ShippingInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship: Order -> ShippingInfos
        modelBuilder.Entity<Order>()
            .HasMany(o => o.ShippingInfos)
            .WithOne(si => si.Order)
            .HasForeignKey(si => si.OrderId);

        // Global query filters to exclude soft-deleted records
        modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<ShippingInfo>().HasQueryFilter(si => !si.IsDeleted);
    }
}
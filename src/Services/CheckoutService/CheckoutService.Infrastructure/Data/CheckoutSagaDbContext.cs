using CheckoutService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Infrastructure.Data;

public class CheckoutSagaDbContext : DbContext
{
    public CheckoutSagaDbContext(DbContextOptions<CheckoutSagaDbContext> options)
        : base(options)
    {
    }

    public DbSet<CheckoutSaga> CheckoutSagas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckoutSaga>(entity =>
        {
            entity.ToTable("CheckoutSagas");
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.State).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.LastUpdatedAt).IsRequired();
            entity.Property(e => e.LastError).IsRequired(false);
        });
    }
}
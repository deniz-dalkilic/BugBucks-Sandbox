using CheckoutService.Domain.Entities;
using CheckoutService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Infrastructure.Data;

public class CheckoutSagaDbContext : DbContext
{
    public CheckoutSagaDbContext(DbContextOptions<CheckoutSagaDbContext> options)
        : base(options)
    {
    }

    public DbSet<CheckoutSaga> CheckoutSagas { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

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
            })
            .Entity<OutboxMessage>(entity =>
            {
                entity.ToTable("OutboxMessages");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.AggregateType).IsRequired();
                entity.Property(x => x.AggregateId).IsRequired();
                entity.Property(x => x.Type).IsRequired();
                entity.Property(x => x.Content).IsRequired();
                entity.Property(x => x.OccurredAt).IsRequired();
                entity.Property(x => x.Processed).HasDefaultValue(false);
            });
    }
}
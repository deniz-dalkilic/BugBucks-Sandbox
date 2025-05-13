using CheckoutService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Infrastructure.Data;

public class CheckoutOutboxDbContext : DbContext
{
    public CheckoutOutboxDbContext(DbContextOptions<CheckoutOutboxDbContext> options)
        : base(options)
    {
    }


    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
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
using CheckoutService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderService.Infrastructure.Data;

/// <summary>
///     Design-time factory for CheckoutOutboxDbContext to support EF Core migrations.
/// </summary>
public class CheckoutOutboxDbContextFactory : IDesignTimeDbContextFactory<CheckoutOutboxDbContext>
{
    public CheckoutOutboxDbContext CreateDbContext(string[] args)
    {
        // Build configuration manually from appsettings.json (ensure the file is copied to output)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CheckoutService.Api"))
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CheckoutOutboxDbContext>();


        var connectionString = configuration.GetConnectionString("OutboxDbConnection");

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new CheckoutOutboxDbContext(optionsBuilder.Options);
    }
}
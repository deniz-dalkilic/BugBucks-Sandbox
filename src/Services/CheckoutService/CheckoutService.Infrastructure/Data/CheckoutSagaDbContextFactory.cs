using CheckoutService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderService.Infrastructure.Data;

/// <summary>
///     Design-time factory for CheckoutSagaDbContext to support EF Core migrations.
/// </summary>
public class CheckoutSagaDbContextFactory : IDesignTimeDbContextFactory<CheckoutSagaDbContext>
{
    public CheckoutSagaDbContext CreateDbContext(string[] args)
    {
        // Build configuration manually from appsettings.json (ensure the file is copied to output)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CheckoutSagaDbContext>();


        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new CheckoutSagaDbContext(optionsBuilder.Options);
    }
}
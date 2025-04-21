using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CheckoutService.Infrastructure.Data;

/// <summary>
///     Design-time factory for CheckoutDbContext to support EF Core migrations.
/// </summary>
public class CheckoutDbContextFactory : IDesignTimeDbContextFactory<CheckoutDbContext>
{
    public CheckoutDbContext CreateDbContext(string[] args)
    {
        // Build configuration manually from appsettings.json (ensure the file is copied to output)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CheckoutDbContext>();

        // Get connection string from configuration ("DefaultConnection")
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new CheckoutDbContext(optionsBuilder.Options);
    }
}
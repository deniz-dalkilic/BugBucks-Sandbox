using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CheckoutService.Infrastructure.Data;

/// <summary>
///     Design-time factory for CheckoutSagaDbContext to support EF Core CLI tools.
/// </summary>
public class CheckoutSagaDbContextFactory : IDesignTimeDbContextFactory<CheckoutSagaDbContext>
{
    public CheckoutSagaDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CheckoutService.Api"))
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var connectionString = configuration.GetConnectionString("SagaDbConnection");

        var optionsBuilder = new DbContextOptionsBuilder<CheckoutSagaDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new CheckoutSagaDbContext(optionsBuilder.Options);
    }
}
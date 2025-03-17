using IdentityService.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Data;

/// <summary>
///     EF Core context for Identity, integrating ASP.NET Core Identity with ApplicationUser.
///     Uses Pomelo.EntityFrameworkCore.MySql for MySQL database.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    ///     Initializes a new instance of the ApplicationDbContext class.
    ///     The connection string is provided via DbContextOptions from the configuration (DefaultConnection).
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    ///     Configures the schema needed for ASP.NET Core Identity.
    /// </summary>
    /// <param name="builder">The builder used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Call the base method to configure Identity tables.
        base.OnModelCreating(builder);

        // Additional configuration can be added here if needed.
    }
}
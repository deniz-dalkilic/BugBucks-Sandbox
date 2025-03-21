using Microsoft.Extensions.DependencyInjection;

namespace BugBucks.Shared.VaultClient.Extensions;

/// <summary>
///     Provides extension methods to register Vault client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers the Vault client service for dependency injection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddVaultClient(this IServiceCollection services)
    {
        services.AddSingleton<IVaultClientService, VaultClientService>();
        return services;
    }
}
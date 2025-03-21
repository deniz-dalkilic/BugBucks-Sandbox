namespace BugBucks.Shared.VaultClient;

/// <summary>
///     Abstraction for a Vault client service that retrieves secrets.
/// </summary>
public interface IVaultClientService
{
    /// <summary>
    ///     Asynchronously retrieves secrets from the specified mount point and secret path.
    /// </summary>
    /// <param name="mountPoint">The Vault mount point (e.g., "identityservice").</param>
    /// <param name="secretPath">The relative secret path (e.g., "dev").</param>
    /// <returns>A dictionary containing secret key-value pairs.</returns>
    Task<IDictionary<string, string>> GetSecretsAsync(string mountPoint, string secretPath);
}
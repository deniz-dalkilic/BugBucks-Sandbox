using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace BugBucks.Shared.Vault;

/// <summary>
///     Provides methods to load secrets from HashiCorp Vault and merge them into an IConfiguration.
/// </summary>
public static class VaultConfigurationProvider
{
    /// <summary>
    ///     Asynchronously loads Vault secrets from the specified path and merges them into the provided configuration.
    /// </summary>
    /// <param name="configuration">The configuration to update.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task LoadSecretsAsync(IConfiguration configuration)
    {
        // Read Vault settings from configuration
        var vaultAddress = configuration["Vault:Address"];
        var vaultToken = configuration["Vault:Token"];
        var vaultSecretPath = configuration["Vault:SecretPath"] ?? "secret/data/identityservice/kv/dev";

        if (string.IsNullOrEmpty(vaultAddress) || string.IsNullOrEmpty(vaultToken))
        {
            Console.WriteLine("Vault Address or Token not provided. Skipping Vault secrets loading.");
            return;
        }

        try
        {
            // Create a Vault client
            var vaultClientSettings = new VaultClientSettings(vaultAddress, new TokenAuthMethodInfo(vaultToken));
            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            var secretResponse = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(vaultSecretPath);
            foreach (var kv in secretResponse.Data.Data)
                // Merge each secret key/value into the configuration.
                // Note: This approach works if the configuration is backed by an in-memory provider.
                configuration[kv.Key] = kv.Value?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading Vault secrets: " + ex.Message);
        }
    }
}
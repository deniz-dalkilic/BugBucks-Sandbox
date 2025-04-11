using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace BugBucks.Shared.VaultClient.Services;

/// <summary>
///     A Vault client service that retrieves secrets from HashiCorp Vault using VaultSharp.
/// </summary>
public class VaultClientService : IVaultClientService
{
    private readonly IVaultClient _vaultClient;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VaultClientService" /> class using configuration.
    /// </summary>
    /// <param name="configuration">The application configuration containing Vault settings.</param>
    public VaultClientService(IConfiguration configuration)
    {
        var vaultAddress = configuration["Vault:Address"];
        var vaultToken = configuration["Vault:Token"];

        if (string.IsNullOrEmpty(vaultAddress) || string.IsNullOrEmpty(vaultToken))
            throw new ArgumentException("Vault Address and Token must be provided in configuration.");

        var vaultClientSettings = new VaultClientSettings(vaultAddress, new TokenAuthMethodInfo(vaultToken));
        _vaultClient = new VaultSharp.VaultClient(vaultClientSettings);
    }

    public async Task<IDictionary<string, string>> GetSecretsAsync(string mountPoint, string secretPath)
    {
        if (string.IsNullOrEmpty(mountPoint))
            throw new ArgumentException("Mount point must be provided.", nameof(mountPoint));
        if (string.IsNullOrEmpty(secretPath))
            throw new ArgumentException("Secret path must be provided.", nameof(secretPath));

        try
        {
            // Read secrets from Vault (KV v2) using mountPoint and secretPath.
            var secretResponse =
                await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath, null, mountPoint);
            IDictionary<string, object> secrets = secretResponse.Data.Data;

            // Vault KV v2 stores the actual secret data under a "data" key.
            /*if (secrets.ContainsKey("data") &&
                secrets["data"] is JsonElement element &&
                element.ValueKind == JsonValueKind.Object)
            {
                var parsedSecrets = new Dictionary<string, string>();
                foreach (var property in element.EnumerateObject())
                    parsedSecrets[property.Name] = property.Value.ToString();
                return parsedSecrets;
            }*/

            // Convert the object values to strings.
            return secrets.ToDictionary(k => k.Key, k => k.Value?.ToString());
        }
        catch (Exception ex)
        {
            // Log error as needed.
            throw new ApplicationException(
                $"Error retrieving Vault secrets for mount point '{mountPoint}' and secret path '{secretPath}'.", ex);
        }
    }
}
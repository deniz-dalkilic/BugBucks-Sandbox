using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace BugBucks.Shared.Vault.Services;

public class VaultService : IVaultService
{
    private readonly IVaultClient _vaultClient;


    public VaultService(IConfiguration configuration)
    {
        var vaultAddress = configuration["Vault:Address"];
        var vaultToken = configuration["Vault:Token"];

        if (string.IsNullOrEmpty(vaultAddress) || string.IsNullOrEmpty(vaultToken))
            throw new ArgumentException("Vault Address and Token must be provided in configuration.");

        var vaultClientSettings = new VaultClientSettings(vaultAddress, new TokenAuthMethodInfo(vaultToken));
        _vaultClient = new VaultClient(vaultClientSettings);
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
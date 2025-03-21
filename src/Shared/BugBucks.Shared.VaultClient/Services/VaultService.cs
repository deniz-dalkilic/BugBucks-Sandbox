using BugBucks.Shared.VaultClient.Config;
using BugBucks.Shared.VaultClient.Services;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace BugBucks.Shared.VaultClient;

public class VaultService : IVaultService
{
    private readonly IVaultClient _vaultClient;

    public VaultService(VaultConfig config)
    {
        var authMethod = new TokenAuthMethodInfo(config.Token);

        var vaultClientSettings = new VaultClientSettings(config.VaultAddress, authMethod);

        _vaultClient = new VaultSharp.VaultClient(vaultClientSettings);
    }

    public async Task<Dictionary<string, object>> GetSecretAsync(string path)
    {
        var secret = await _vaultClient.V1.Secrets.KeyValue.V2
            .ReadSecretAsync(path);

        return secret?.Data?.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
    }
}
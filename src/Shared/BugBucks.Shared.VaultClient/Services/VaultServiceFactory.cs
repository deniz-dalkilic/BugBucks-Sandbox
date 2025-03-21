using BugBucks.Shared.VaultClient.Config;
using BugBucks.Shared.VaultClient.Services;

namespace BugBucks.Shared.VaultClient
{
    public static class VaultServiceFactory
    {
        public static IVaultService CreateFromEnvironment()
        {
            var vaultAddress = Environment.GetEnvironmentVariable("VAULT_ADDR");
            var vaultToken = Environment.GetEnvironmentVariable("VAULT_TOKEN");

            if (string.IsNullOrEmpty(vaultAddress) || string.IsNullOrEmpty(vaultToken))
                throw new InvalidOperationException("Vault configuration not found in environment variables.");

            var config = new VaultConfig
            {
                VaultAddress = vaultAddress,
                Token = vaultToken
            };

            return new VaultService(config);
        }
    }
}
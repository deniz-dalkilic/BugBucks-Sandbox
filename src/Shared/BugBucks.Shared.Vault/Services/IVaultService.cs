namespace BugBucks.Shared.Vault.Services;

public interface IVaultService
{
    Task<IDictionary<string, string>> GetSecretsAsync(string mountPoint, string secretPath);
}
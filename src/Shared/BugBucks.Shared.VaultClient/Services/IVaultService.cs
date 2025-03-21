namespace BugBucks.Shared.VaultClient.Services;

public interface IVaultService
{
    Task<Dictionary<string, object>> GetSecretAsync(string path);
}
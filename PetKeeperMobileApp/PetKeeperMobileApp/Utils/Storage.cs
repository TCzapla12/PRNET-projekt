using Grpc.Core;

namespace PetKeeperMobileApp.Utils;

public class Storage
{
    public static async Task<Metadata> GetMetadata()
    {
        return new Metadata
        {
            { "Authorization", $"Bearer { await Storage.GetToken() }" }
        };
    }

    public static async Task SaveCredentials(string email, string token)
    {
        await SecureStorage.SetAsync("email", email);
        await SecureStorage.SetAsync("token", token);
    }

    public static void RemoveCredentials()
    {
        SecureStorage.Remove("email");
        SecureStorage.Remove("token");
    }

    public static async Task<string> GetToken()
    {
        return await SecureStorage.GetAsync("token") ?? string.Empty;
    }

    public static async Task<string> GetUserEmail()
    {
        return await SecureStorage.GetAsync("email") ?? string.Empty;
    }
}

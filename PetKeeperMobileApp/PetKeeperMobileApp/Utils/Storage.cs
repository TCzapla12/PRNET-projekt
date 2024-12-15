namespace PetKeeperMobileApp.Utils;

public class Storage
{
    public static async Task SaveToken(string token)
    {
        await SecureStorage.SetAsync("token", token);
    }

    public static async Task<string> LoadToken()
    {
        return await SecureStorage.GetAsync("token") ?? string.Empty;
    }

    public static void RemoveToken()
    {
        SecureStorage.Remove("token");
    }
}

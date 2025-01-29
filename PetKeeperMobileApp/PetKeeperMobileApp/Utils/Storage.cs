using Grpc.Core;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
        await SecureStorage.SetAsync("id", await GetUserIdFromToken(token));
        await SecureStorage.SetAsync("email", email);
        await SecureStorage.SetAsync("token", token);
    }

    public static void RemoveCredentials()
    {
        SecureStorage.Remove("id");
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

    public static async Task<string> GetUserId()
    {
        return await SecureStorage.GetAsync("id") ?? string.Empty;
    }

    private static async Task<string> GetUserIdFromToken(string token)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        if (jwtHandler.CanReadToken(token))
        {
            var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken != null)
            {
                var claims = jwtToken.Claims;
                var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                return claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value!;
            }
        }
        return string.Empty;
    }
}

using Grpc.Net.Client;
using PetKeeperMobileApp.Models;

namespace PetKeeperMobileApp.Services;

public class GrpcClient
{
    private readonly static string host = "localhost";
    private readonly static string port = "7042";

    public static async Task<string> Login(LoginDto loginDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new Auth.AuthClient(channel);
        var reply = await client.LoginAsync(new LoginRequest { Email = loginDto.Email, PasswordHash = loginDto.HashPassword });
        return reply.Token;
    }
}

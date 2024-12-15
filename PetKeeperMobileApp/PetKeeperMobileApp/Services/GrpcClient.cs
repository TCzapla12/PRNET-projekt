using Grpc.Net.Client;
using PetKeeperMobileApp.Models;

namespace PetKeeperMobileApp.Services;

public class GrpcClient : IGrpcClient
{
    private readonly static string host = "localhost";
    private readonly static string port = "7042";

    public async Task<string> Login(AuthDto authDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AuthService.AuthServiceClient(channel);
        var reply = await client.AuthenticateAsync(new AuthRequest { Email = authDto.Email, Password = authDto.HashPassword });
        return reply.Token;
    }

    public async Task<string> ResetPassword(string email)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new UserService.UserServiceClient(channel);
        var reply = await client.ResetPasswordAsync(new ResetPasswordRequest { Email = email });
        return reply.Message;
    }
}

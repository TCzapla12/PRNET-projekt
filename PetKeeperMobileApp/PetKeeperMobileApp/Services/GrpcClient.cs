using Grpc.Net.Client;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Services;

public class GrpcClient : IGrpcClient
{
    private readonly static string host = "10.0.2.2";
    private readonly static string port = "8080";

    public async Task<string> Login(AuthDto authDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AuthService.AuthServiceClient(channel);
        var reply = await client.AuthenticateAsync(new AuthRequest { UserId = new UserIdentifier() { Email = authDto.Email }, Password = authDto.HashPassword });
        return reply.Token;
    }

    public async Task<string> ResetPassword(string email)
    {
        //throw new NotImplementedException();
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new UserService.UserServiceClient(channel);
        var reply = await client.ResetPasswordAsync(new ResetPasswordRequest { Email = email });
        return Wordings.RESET_PASSWORD_SUCCESS;
    }

    public async Task<string> Register(RegisterDto registerDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new UserService.UserServiceClient(channel);
        AddressCreate address = new()
        {
            Street = registerDto.PrimaryAddress.Street,
            HouseNumber = registerDto.PrimaryAddress.HouseNumber,
            ApartmentNumber = registerDto.PrimaryAddress.ApartmentNumber,
            City = registerDto.PrimaryAddress.City,
            PostCode = registerDto.PrimaryAddress.ZipCode,
            //Description = string.Empty
        };
        UserCreate user = new()
        {
            Email = registerDto.Email,
            Username = registerDto.Username,
            Password = registerDto.HashPassword,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Phone = registerDto.Phone,
            Pesel = registerDto.Pesel,
            AvatarUrl = registerDto.AvatarUrl,
            PrimaryAddress = address
        };
        user.DocumentUrl.AddRange(registerDto.DocumentUrls);
        var reply = await client.CreateUserAsync(user);
        return Wordings.REGISTER_SUCCESS;
    }
}

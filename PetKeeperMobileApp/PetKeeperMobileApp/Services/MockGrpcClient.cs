using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Services;

public class MockGrpcClient : IGrpcClient
{
    public async Task<string> Login(AuthDto authDto)
    {
        return "1234";
    }

    public async Task<string> ResetPassword(string email)
    {
        return Wordings.RESET_PASSWORD_SUCCESS;
    }

    public async Task<string> Register(RegisterDto registerDto)
    {
        return Wordings.REGISTER_SUCCESS;
    }
}

using PetKeeperMobileApp.Models;

namespace PetKeeperMobileApp.Services;

public interface IGrpcClient
{
    Task<string> Login(AuthDto authDto);

    Task<string> ResetPassword(string email);
}

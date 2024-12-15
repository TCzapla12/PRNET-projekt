using PetKeeperMobileApp.Models;

namespace PetKeeperMobileApp.Services;

public class MockGrpcClient : IGrpcClient
{
    public async Task<string> Login(AuthDto authDto)
    {
        return "1234";
    }

    public async Task<string> ResetPassword(string email)
    {
        return "Na twój adres e-mail została wysłana wiadomość umożliwiająca zmianę hasła.";
    }
}

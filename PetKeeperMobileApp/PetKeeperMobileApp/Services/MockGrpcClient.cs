using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Services;

public class MockGrpcClient : IGrpcClient
{
    #region Auth
    public async Task<CredentialsDto> Login(AuthDto authDto)
    {
        return new CredentialsDto 
        {
            Token = "1234",
            Id = "12AA"
        };
    }
    #endregion

    #region User
    public async Task<string> ResetPassword(string email)
    {
        return Wordings.RESET_PASSWORD_SUCCESS;
    }

    public async Task<string> Register(RegisterDto registerDto)
    {
        return Wordings.REGISTER_SUCCESS;
    }
    #endregion

    #region Address
    public async Task<List<AddressDto>> GetAddresses()
    {
        return [
            new AddressDto() {
                Street = "Abackiego",
                HouseNumber = "1",
                ApartmentNumber = "12",
                City = "Warszawa",
                ZipCode = "03-318",
                IsPrimary = true,
            },
            new AddressDto() {
                Street = "Babackiego",
                HouseNumber = "7",
                City = "Wołomin",
                ZipCode = "05-200"
            }
            ];
    }

    public Task<string> CreateAddress(AddressDto addressDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateAddress(AddressDto addressDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteAddress(string id)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Animal
    public async Task<List<AnimalDto>> GetAnimals()
    {
        return [
            new AnimalDto() {
                Name = "Ugryź",
                Type = Enums.AnimalType.Dog,
                PhotoUrl = String.Empty,
                Description = "Bardzo fajny piesek :)"
            },
            new AnimalDto() {
                Name = "Kocur",
                Type = Enums.AnimalType.Cat,
                PhotoUrl = String.Empty,
            },
            new AnimalDto() {
                Name = "Cosiek",
                Type = Enums.AnimalType.Other,
                PhotoUrl = String.Empty,
                Description = "Legia Warszawa to najlepszy klub - Mistrz Polski!!! Polska GUUUUROM"
            }
            ];
    }

    public Task<string> CreateAnimal(AnimalDto animalDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateAnimal(AnimalDto animalDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteAnimal(string id)
    {
        throw new NotImplementedException();
    }
    #endregion
}

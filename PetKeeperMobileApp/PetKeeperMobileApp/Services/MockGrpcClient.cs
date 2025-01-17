using PetKeeperMobileApp.Enums;
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
                Id = "1",
                Street = "Abackiego",
                HouseNumber = "1",
                ApartmentNumber = "12",
                City = "Warszawa",
                ZipCode = "03-318",
                IsPrimary = true,
            },
            new AddressDto() {
                Id = "2",
                Street = "Babackiego",
                HouseNumber = "7",
                City = "Wołomin",
                ZipCode = "05-200"
            }
            ];
    }

    public async Task<string> CreateAddress(AddressDto addressDto)
    {
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateAddress(AddressDto addressDto)
    {
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAddress(string id)
    {
        return Wordings.SUCCESS;
    }
    #endregion

    #region Animal
    public async Task<List<AnimalDto>> GetAnimals()
    {
        ImageSource imageSource = "/data/data/com.prnet.petkeepermobileapp/cache/2203693cc04e0be7f4f024d5f9499e13/821da731fa9143d2b6e7cfe4f7c954ee/1000000034.png";
        var bytes = await Helpers.ImageToBytes(imageSource);
        return [
            new AnimalDto() {
                Id = "1",
                Name = "Ugryź",
                Type = AnimalType.Dog,
                Photo = bytes,
                Description = "Bardzo fajny piesek :)"
            },
            new AnimalDto() {
                Id = "2",
                Name = "Kocur",
                Type = AnimalType.Cat,
                Photo = [],
            },
            new AnimalDto() {
                Id = "3",
                Name = "Cosiek",
                Type = AnimalType.Other,
                Photo = [],
                Description = "Legia Warszawa to najlepszy klub - Mistrz Polski!!! Polska GUUUUROM"
            }
            ];
    }

    public async Task<string> CreateAnimal(AnimalDto animalDto)
    {
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateAnimal(AnimalDto animalDto)
    {
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAnimal(string id)
    {
        return Wordings.SUCCESS;
    }
    #endregion
}

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
            Token = "1234"
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

    public async Task<UserDto> GetUser()
    {
        ImageSource imageSource = "/data/data/com.prnet.petkeepermobileapp/cache/2203693cc04e0be7f4f024d5f9499e13/821da731fa9143d2b6e7cfe4f7c954ee/1000000034.png";
        var bytes = await Helpers.ImageToBytes(imageSource);
        return new UserDto
        {
            Id = "1",
            Email = "a@a.com",
            Username = "Test",
            FirstName = "Artur",
            LastName = "Abacki",
            Phone = "123123123",
            Pesel = "99999900001",
            Photo = bytes,
            PrimaryAddress = new AddressDto
            {
                Id = "1",
                Street = "Abackiego",
                HouseNumber = "1",
                ApartmentNumber = "12",
                City = "Warszawa",
                ZipCode = "03-318",
                IsPrimary = true
            }
        };
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

    public async Task<AddressDto> GetAddress(string id)
    {
        return new AddressDto()
        {
            Id = "1",
            Street = "Abackiego",
            HouseNumber = "1",
            ApartmentNumber = "12",
            City = "Warszawa",
            ZipCode = "03-318",
            IsPrimary = true,
        };
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

    public async Task<AnimalDto> GetAnimal(string id)
    {
        return new AnimalDto()
        {
            Id = "1",
            Name = "Ugryź",
            Type = AnimalType.Dog,
            Photo = [],
            Description = "Bardzo fajny piesek :)"
        };
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

    #region Announcement
    public async Task<List<AnnouncementDto>> GetAnnouncements()
    {
        return [
            new AnnouncementDto() {
                Id = "1",
                AnimalId = "1",
                Profit = 100,
                IsNegotiable = true,
                Description = "Bardzo dobre ogłoszenie",
                StartTerm = (ulong)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
                EndTerm = (ulong)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Status = StatusType.Created,
                AddressId = "1",
            },
            new AnnouncementDto() {
                Id = "2",
                AnimalId = "2",
                Profit = 200,
                IsNegotiable = false,
                StartTerm = (ulong)new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds(),
                EndTerm = (ulong)new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds(),
                Status = StatusType.Ongoing,
                AddressId = "2",
            },
            new AnnouncementDto() {
                Id = "3",
                AnimalId = "3",
                Profit = 300,
                IsNegotiable = false,
                Description = "Legia Warszawa to najlepszy klub - Mistrz Polski!!! Polska GUUUUROM",
                StartTerm = (ulong)new DateTimeOffset(DateTime.MinValue).ToUnixTimeSeconds(),
                EndTerm = (ulong)new DateTimeOffset(DateTime.MaxValue).ToUnixTimeSeconds(),
                Status = StatusType.Finished,
                AddressId = "3",
            },
            new AnnouncementDto() {
                Id = "4",
                AnimalId = "3",
                Profit = 300,
                IsNegotiable = false,
                Description = "Legia Warszawa to najlepszy klub - Mistrz Polski!!! Polska GUUUUROM",
                StartTerm = (ulong)new DateTimeOffset(DateTime.MinValue).ToUnixTimeSeconds(),
                EndTerm = (ulong)new DateTimeOffset(DateTime.MaxValue).ToUnixTimeSeconds(),
                Status = StatusType.Pending,
                AddressId = "4",
            },
            new AnnouncementDto() {
                Id = "5",
                AnimalId = "3",
                Profit = 300,
                IsNegotiable = false,
                Description = "Legia Warszawa to najlepszy klub - Mistrz Polski!!! Polska GUUUUROM",
                StartTerm = (ulong)new DateTimeOffset(DateTime.MinValue).ToUnixTimeSeconds(),
                EndTerm = (ulong)new DateTimeOffset(DateTime.MaxValue).ToUnixTimeSeconds(),
                Status = StatusType.Canceled,
                AddressId = "4",
            },
            new AnnouncementDto() {
                Id = "6",
                AnimalId = "3",
                Profit = 300,
                IsNegotiable = false,
                Description = "Legia Warszawa to najlepszy klub - Mistrz Polski!!! Polska GUUUUROM",
                StartTerm = (ulong)new DateTimeOffset(DateTime.MinValue).ToUnixTimeSeconds(),
                EndTerm = (ulong)new DateTimeOffset(DateTime.MaxValue).ToUnixTimeSeconds(),
                Status = StatusType.Accepted,
                AddressId = "4",
            }
            ];
    }

    public async Task<string> CreateAnnouncement(AnnouncementDto announcementDto)
    {
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateAnnouncement(AnnouncementDto announcementDto)
    {
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAnnouncement(string id)
    {
        return Wordings.SUCCESS;
    }
    #endregion
}

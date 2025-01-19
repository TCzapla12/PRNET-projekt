using PetKeeperMobileApp.Models;

namespace PetKeeperMobileApp.Services;

public interface IGrpcClient
{
    #region Auth
    Task<CredentialsDto> Login(AuthDto authDto);
    #endregion

    #region User
    Task<string> ResetPassword(string email);

    Task<string> Register(RegisterDto registerDto);
    #endregion

    #region Address
    Task<List<AddressDto>> GetAddresses();

    Task<AddressDto> GetAddress(string id);

    Task<string> CreateAddress(AddressDto addressDto);

    Task<string> UpdateAddress(AddressDto addressDto);

    Task<string> DeleteAddress(string id);
    #endregion

    #region Animal
    Task<List<AnimalDto>> GetAnimals();

    Task<AnimalDto> GetAnimal(string id);

    Task<string> CreateAnimal(AnimalDto animalDto);

    Task<string> UpdateAnimal(AnimalDto animalDto);

    Task<string> DeleteAnimal(string id);
    #endregion

    #region Announcement
    Task<List<AnnouncementDto>> GetAnnouncements();

    Task<string> CreateAnnouncement(AnnouncementDto announcementDto);

    Task<string> UpdateAnnouncement(AnnouncementDto announcementDto);

    Task<string> DeleteAnnouncement(string id);
    #endregion
}

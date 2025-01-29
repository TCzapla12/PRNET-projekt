using PetKeeperMobileApp.Enums;
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

    Task<UserDto> GetUser(string? id = null);
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
    Task<List<AnnouncementDto>> GetUserAnnouncements(StatusType? status = null);

    Task<List<AnnouncementDto>> GetAnnouncements(int? minValue = null, int? maxValue = null,
        DateTime? startTerm = null, DateTime? endTerm = null, string? keeperId = null, StatusType? status = null);

    Task<string> CreateAnnouncement(AnnouncementDto announcementDto);

    Task<string> UpdateAnnouncement(AnnouncementDto announcementDto);

    Task<string> UpdateAnnouncementStatus(UpdateAnnouncementDto announcementUpdateDto);

    Task<string> DeleteAnnouncement(string id);
    #endregion

    #region Opinions
    Task<List<OpinionDto>> GetOpinions(string? authorId = null, string? keeperId = null);

    Task<string> CreateOpinion(OpinionDto opinionDto);

    Task<string> UpdateOpinion(OpinionDto opinionDto);

    Task<string> DeleteOpinion(string id);
    #endregion
}

namespace PetKeeperMobileApp.Models;

public class OpinionInfo
{
    public string Id { get; set; }

    public string AuthorId { get; set; }

    public string KeeperId { get; set; }

    public string? Description { get; set; }

    public ulong CreatedDate { get; set; }

    public uint Rating { get; set; }

    public string AnnouncementId { get; set; }

    public UserInfo? AuthorInfo { get; set; }

    public UserInfo? KeeperInfo { get; set; }
}

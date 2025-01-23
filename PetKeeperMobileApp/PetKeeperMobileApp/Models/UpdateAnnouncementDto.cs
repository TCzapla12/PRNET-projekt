using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.Models;

public class UpdateAnnouncementDto
{
    public required string Id { get; set; }

    public required StatusType Status { get; set; }

    public string? KeeperId { get; set; }
}

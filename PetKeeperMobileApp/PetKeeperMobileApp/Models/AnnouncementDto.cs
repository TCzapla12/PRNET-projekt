using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.Models;

public class AnnouncementDto
{
    public string? Id { get; set; }

    public required string AnimalId { get; set; }

    public required uint Profit { get; set; }

    public required bool IsNegotiable { get; set; }

    public string? Description { get; set; }

    public required ulong StartTerm { get; set; }

    public required ulong EndTerm { get; set; }

    public required StatusType Status { get; set; }

    public required string AddressId { get; set; }

    public string? OwnerId { get; set; }

    public string? KeeperId { get; set; }
}

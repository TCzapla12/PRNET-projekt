using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.Models;

public class AnnouncementInfo
{
    public string? Id { get; set; }

    public string Animal { get; set; }

    public uint Profit { get; set; }

    public bool IsNegotiable { get; set; }

    public string? Description { get; set; }

    public ulong StartTerm { get; set; }

    public ulong EndTerm { get; set; }

    public StatusType Status { get; set; }

    public string Address { get; set; }

    public string? Owner { get; set; }

    public string? Keeper { get; set; }

    public UserInfo? OwnerInfo { get; set; }

    public UserInfo? KeeperInfo { get; set; }

    public AnimalInfo? AnimalInfo { get; set; }

    public AddressInfo? AddressInfo { get; set; }
}

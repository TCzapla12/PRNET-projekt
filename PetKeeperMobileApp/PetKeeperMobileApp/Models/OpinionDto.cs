namespace PetKeeperMobileApp.Models;

public class OpinionDto
{
    public string? Id { get; set; }

    public required string AuthorId { get; set; }

    public required string KeeperId { get; set; }

    public string? Description { get; set; }

    public required ulong CreatedDate { get; set; }

    public required uint Rating { get; set; }

    public required string AnnouncementId { get; set; }
}

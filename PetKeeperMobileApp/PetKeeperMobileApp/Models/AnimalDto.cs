using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.Models;

public class AnimalDto
{
    public required string Name {  get; set; }
    
    public required AnimalType Type { get; set; }

    public required string PhotoUrl { get; set; }

    public string? Description { get; set; }
}

using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.Models;

public class AnimalDto
{
    public string? Id { get; set; }

    public required string Name {  get; set; }
    
    public required AnimalType Type { get; set; }

    public required byte[] Photo { get; set; }

    public string? Description { get; set; }
}

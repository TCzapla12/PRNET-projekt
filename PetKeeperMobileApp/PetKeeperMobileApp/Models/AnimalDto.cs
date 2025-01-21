using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Models;

public class AnimalDto
{
    public string? Id { get; set; }

    public required string Name {  get; set; }
    
    public required AnimalType Type { get; set; }

    public required byte[] Photo { get; set; }

    public string? Description { get; set; }

    public static string AnimalToString(AnimalDto animalDto)
    {
        var text = animalDto.Name + " (" + Helpers.GetDescription(animalDto.Type) + ")";
        if (!string.IsNullOrWhiteSpace(animalDto.Description))
            text += "\n" + animalDto.Description;
        return text;
    }
}

using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.Models;

public class AnimalInfo
{
    public AnimalInfo(int index, AnimalDto animalDto)
    {
        Index = index;
        Name = animalDto.Name;
        Type = animalDto.Type;
        PhotoUrl = animalDto.PhotoUrl;
        Description = animalDto.Description ?? String.Empty;
    }
    public int Index { get; set; }

    public string Name { get; set; }

    public AnimalType Type { get; set; }

    public string PhotoUrl { get; set; }

    public string Description { get; set; }
}

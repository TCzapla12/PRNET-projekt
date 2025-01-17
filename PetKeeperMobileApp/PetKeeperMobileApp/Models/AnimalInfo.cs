using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Models;

public class AnimalInfo
{
    public AnimalInfo(AnimalDto animalDto)
    {
        Id = animalDto.Id;
        Name = animalDto.Name;
        Type = animalDto.Type;
        Photo = Helpers.AnimalBytesToImage(animalDto.Photo, animalDto.Type);
        Description = animalDto.Description ?? String.Empty;
    }
    public string Id { get; set; }

    public string Name { get; set; }

    public AnimalType Type { get; set; }

    public ImageSource Photo { get; set; }

    public string Description { get; set; }
}

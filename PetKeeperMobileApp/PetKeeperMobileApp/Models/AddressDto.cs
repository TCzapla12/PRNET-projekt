namespace PetKeeperMobileApp.Models;

public class AddressDto
{
    public string? Id { get; set; }

    public required string Street { get; set; }

    public required string HouseNumber { get; set; }

    public string? ApartmentNumber { get; set; }

    public required string City { get; set; }

    public required string ZipCode { get; set; }

    public bool? IsPrimary { get; set; }

    public static string AddressToString(AddressDto addressDto)
    {
        var text = addressDto.Street + " " + addressDto.HouseNumber;
        if (!string.IsNullOrWhiteSpace(addressDto.ApartmentNumber))
            text += "/" + addressDto.ApartmentNumber;
        text += "\n" + addressDto.ZipCode + ", " + addressDto.City;
        return text;
    }
}

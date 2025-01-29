namespace PetKeeperMobileApp.Models;

public class AddressInfo
{
    public AddressInfo(int index, AddressDto addressDto) : this(addressDto)
    {
        Index = index;
    }

    public AddressInfo(AddressDto addressDto)
    {
        Id = addressDto.Id;
        Address1 = addressDto.Street + " " + addressDto.HouseNumber;
        if (!string.IsNullOrWhiteSpace(addressDto.ApartmentNumber))
            Address1 += "/" + addressDto.ApartmentNumber;
        Address2 = addressDto.ZipCode + ", " + addressDto.City;
    }
    public string Id { get; set; }

    public int Index { get; set; }
    
    public string Address1 { get; set; }

    public string Address2 { get; set; }
}

namespace PetKeeperMobileApp.Models;

public class AddressInfo
{
    public AddressInfo(int index, AddressDto addressDto)
    {
        Index = index;
        Address1 = addressDto.Street + " " + addressDto.HouseNumber;
        if (!string.IsNullOrWhiteSpace(addressDto.ApartmentNumber))
            Address1 += "/" + addressDto.ApartmentNumber;
        Address2 = addressDto.ZipCode + ", " + addressDto.City;
    }
    public int Index { get; set; }
    
    public string Address1 { get; set; }

    public string Address2 { get; set; }
}

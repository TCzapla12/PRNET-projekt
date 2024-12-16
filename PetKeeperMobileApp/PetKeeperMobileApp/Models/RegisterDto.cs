namespace PetKeeperMobileApp.Models;

public class RegisterDto
{
    public required string Email { get; set; }

    public required string Username { get; set; }

    public required string HashPassword { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required AddressDto PrimaryAddress { get; set; }

    public required string Phone { get; set; }

    public required string Pesel { get; set; }

    public required string AvatarUrl { get; set; }

    public required List<string> DocumentUrls { get; set; }
}

public class AddressDto
{
    public required string Street { get; set; }

    public required string HouseNumber { get; set; }

    public string? ApartmentNumber { get; set; }

    public required string City { get; set; }

    public required string ZipCode { get; set; }
}

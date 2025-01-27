namespace PetKeeperMobileApp.Models;

public class UserDto
{
    public required string Id { get; set; }

    public required string Email { get; set; }

    public required string Username { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Phone { get; set; }

    public required string Pesel { get; set; }

    public required byte[] Photo { get; set; }

    public required AddressDto PrimaryAddress { get; set; }
}

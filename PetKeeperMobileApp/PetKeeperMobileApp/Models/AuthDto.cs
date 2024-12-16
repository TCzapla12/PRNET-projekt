namespace PetKeeperMobileApp.Models;

public class AuthDto
{
    public required string Email { get; set; }

    public required string HashPassword { get; set; }
}

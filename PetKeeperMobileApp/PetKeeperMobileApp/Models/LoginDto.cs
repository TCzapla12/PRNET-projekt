namespace PetKeeperMobileApp.Models;

public class LoginDto
{
    public required string Email { get; set; }

    public required string HashPassword { get; set; }
}

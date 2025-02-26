﻿namespace PetKeeperMobileApp.Models;

public class RegisterDto
{
    public required string Email { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required AddressDto PrimaryAddress { get; set; }

    public required string Phone { get; set; }

    public required string Pesel { get; set; }

    public required byte[] AvatarPng { get; set; }

    public required List<byte[]> DocumentPngs { get; set; }
}

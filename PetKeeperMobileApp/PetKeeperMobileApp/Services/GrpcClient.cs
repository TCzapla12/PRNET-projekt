using Grpc.Net.Client;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Services;

public class GrpcClient : IGrpcClient
{
    private readonly static string host = "10.0.2.2";
    private readonly static string port = "8080";

    #region Auth
    public async Task<CredentialsDto> Login(AuthDto authDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AuthService.AuthServiceClient(channel);
        var reply = await client.AuthenticateAsync(new AuthRequest { UserId = new UserIdentifier() { Email = authDto.Email }, Password = authDto.HashPassword });
        return new CredentialsDto 
        { 
            Token = reply.Token,
            Id = reply.Id,
        };
    }
    #endregion

    #region User
    public async Task<string> ResetPassword(string email)
    {
        //throw new NotImplementedException();
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new UserService.UserServiceClient(channel);
        var reply = await client.ResetPasswordAsync(new ResetPasswordRequest { Email = email });
        return Wordings.RESET_PASSWORD_SUCCESS;
    }

    public async Task<string> Register(RegisterDto registerDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new UserService.UserServiceClient(channel);
        AddressCreate address = new()
        {
            Street = registerDto.PrimaryAddress.Street,
            HouseNumber = registerDto.PrimaryAddress.HouseNumber,
            ApartmentNumber = registerDto.PrimaryAddress.ApartmentNumber,
            City = registerDto.PrimaryAddress.City,
            PostCode = registerDto.PrimaryAddress.ZipCode,
            IsPrimary = true
            //Description = string.Empty
        };
        UserCreate user = new()
        {
            Email = registerDto.Email,
            Username = registerDto.Username,
            Password = registerDto.HashPassword,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Phone = registerDto.Phone,
            Pesel = registerDto.Pesel,
            AvatarUrl = registerDto.AvatarUrl,
            PrimaryAddress = address
        };
        user.DocumentUrl.AddRange(registerDto.DocumentUrls);
        var reply = await client.CreateUserAsync(user);
        return Wordings.REGISTER_SUCCESS;
    }
    #endregion

    #region Address
    public async Task<List<AddressDto>> GetAddresses()
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AddressService.AddressServiceClient(channel);
        var reply = await client.GetUserAddressesAsync(new AddressGet { OwnerId = await Storage.GetUserId()});
        var addresses = new List<AddressDto>();
        foreach (var address in reply.Addresses) 
        {
            addresses.Add(new AddressDto()
            {
                Street = address.Street,
                HouseNumber = address.HouseNumber,
                ApartmentNumber = address.ApartmentNumber,
                City = address.City,
                ZipCode = address.PostCode,
                IsPrimary = address.IsPrimary,
            });
        }
        return addresses;
    }

    public Task<string> CreateAddress(AddressDto addressDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateAddress(AddressDto addressDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteAddress(string id)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Animal
    public async Task<List<AnimalDto>> GetAnimals()
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        var reply = await client.GetAnimalsAsync(new AnimalGet { OwnerId = await Storage.GetUserId() });
        var animals = new List<AnimalDto>();
        foreach (var animal in reply.Animals)
        {
            animals.Add(new AnimalDto()
            {
                Name = animal.Name,
                Type = Enum.TryParse<AnimalType>(animal.Type, out var parsedType) ? parsedType : AnimalType.Other,
                PhotoUrl = animal.Photos.FirstOrDefault(String.Empty),
                Description = animal.Description
            });
        }
        return animals;
    }

    public Task<string> CreateAnimal(AnimalDto animalDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateAnimal(AnimalDto animalDto)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteAnimal(string id)
    {
        throw new NotImplementedException();
    }
    #endregion
}

using Google.Protobuf;
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
            AvatarPng = ByteString.CopyFrom(registerDto.AvatarPng),
            PrimaryAddress = address
        };
        foreach (var documentPng in registerDto.DocumentPngs)
            user.DocumentPngs.Add(ByteString.CopyFrom(documentPng));
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
                Id = address.Id,
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

    public async Task<string> CreateAddress(AddressDto addressDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AddressService.AddressServiceClient(channel);
        AddressCreate address = new()
        {
            Street = addressDto.Street,
            HouseNumber = addressDto.HouseNumber,
            ApartmentNumber = addressDto.ApartmentNumber,
            City = addressDto.City,
            PostCode = addressDto.ZipCode,
        };
        var reply = await client.CreateAddressAsync(address);
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateAddress(AddressDto addressDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AddressService.AddressServiceClient(channel);
        AddressUpdate address = new()
        {
            Id = addressDto.Id,
            Street = addressDto.Street,
            HouseNumber = addressDto.HouseNumber,
            ApartmentNumber = addressDto.ApartmentNumber,
            City = addressDto.City,
            PostCode = addressDto.ZipCode,
        };
        var reply = await client.UpdateAddressAsync(address);
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAddress(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AddressService.AddressServiceClient(channel);
        var reply = await client.DeleteAddressAsync(new AddressMinimal { Id = id, OwnerId = await Storage.GetUserId() });
        return Wordings.SUCCESS;
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
                Id = animal.Id,
                Name = animal.Name,
                Type = Enum.TryParse<AnimalType>(animal.Type, out var parsedType) ? parsedType : AnimalType.Other,
                Photo = animal.Photo.ToByteArray(),
                Description = animal.Description
            });
        }
        return animals;
    }

    public async Task<string> CreateAnimal(AnimalDto animalDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        AnimalCreate animal = new()
        {
            Name = animalDto.Name,
            Type = animalDto.Type.ToString(),
            Photo = ByteString.CopyFrom(animalDto.Photo),
            Description = animalDto.Description
        };
        var reply = await client.CreateAnimalAsync(animal);
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateAnimal(AnimalDto animalDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        AnimalUpdate animal = new()
        {
            Id = animalDto.Id,
            Name = animalDto.Name,
            Type = animalDto.Type.ToString(),
            Photo = ByteString.CopyFrom(animalDto.Photo),
            Description = animalDto.Description
        };
        var reply = await client.UpdateAnimalAsync(animal);
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAnimal(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        var reply = await client.DeleteAnimalAsync(new AnimalMinimal { Id = id, OwnerId = await Storage.GetUserId() });
        return Wordings.SUCCESS;
    }
    #endregion
}

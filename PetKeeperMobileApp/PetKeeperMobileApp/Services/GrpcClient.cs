using Google.Protobuf;
using Grpc.Net.Client;
using grpc_hello_world;
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
            //Id = reply.Id,
        };
    }
    #endregion

    #region User
    public async Task<string> ResetPassword(string email)
    {
        throw new NotImplementedException();
        //using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        //var client = new UserService.UserServiceClient(channel);
        //var reply = await client.ResetPasswordAsync(new ResetPasswordRequest { Email = email });
        //return Wordings.RESET_PASSWORD_SUCCESS;
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

    public async Task<UserDto> GetUser(string? id = null)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}", new GrpcChannelOptions
        {
            MaxReceiveMessageSize = 10 * 1024 * 1024
        });
        var userIdentifier = new UserIdentifier();
        if (id == null) userIdentifier.Email = await Storage.GetUserEmail();
        else userIdentifier.Id = id;
        var client = new UserService.UserServiceClient(channel);
        var reply = await client.GetUserAsync(new UserGet { UserId = userIdentifier }, await Storage.GetMetadata());
        UserDto user = new()
        {
            Id = reply.Id,
            Email = reply.Email,
            Username = reply.Username,
            FirstName = reply.FirstName,
            LastName = reply.LastName,
            Phone = reply.Phone,
            Pesel = reply.Pesel,
            Photo = reply.AvatarPng.ToByteArray(),
            PrimaryAddress = new AddressDto
            {
                Id = reply.PrimaryAddress.Id,
                Street = reply.PrimaryAddress.Street,
                HouseNumber = reply.PrimaryAddress.HouseNumber,
                ApartmentNumber = reply.PrimaryAddress.ApartmentNumber,
                City = reply.PrimaryAddress.City,
                ZipCode = reply.PrimaryAddress.PostCode
            }
        };
        return user;
    }
    #endregion

    #region Address
    public async Task<List<AddressDto>> GetAddresses()
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AddressService.AddressServiceClient(channel);
        var reply = await client.GetAddressesAsync(new AddressGet { OwnerId = String.Empty }, await Storage.GetMetadata());
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

    public async Task<AddressDto> GetAddress(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AddressService.AddressServiceClient(channel);
        var reply = await client.GetAddressesAsync(new AddressGet { Id = id }, await Storage.GetMetadata());
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
        return addresses[0];
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
        var reply = await client.CreateAddressAsync(address, await Storage.GetMetadata());
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
        var reply = await client.UpdateAddressAsync(address, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAddress(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AddressService.AddressServiceClient(channel);
        var reply = await client.DeleteAddressAsync(new AddressMinimal { Id = id }, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }
    #endregion

    #region Animal
    public async Task<List<AnimalDto>> GetAnimals()
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        var reply = await client.GetAnimalsAsync(new AnimalGet { OwnerId = String.Empty }, await Storage.GetMetadata());
        var animals = new List<AnimalDto>();
        foreach (var animal in reply.Animals)
        {
            animals.Add(new AnimalDto()
            {
                Id = animal.Id,
                Name = animal.Name,
                Type = Enum.TryParse<AnimalType>(animal.Type, true, out var parsedType) ? parsedType : AnimalType.Other,
                Photo = animal.Photo.ToByteArray(),
                Description = animal.Description
            });
        }
        return animals;
    }

    public async Task<AnimalDto> GetAnimal(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        var reply = await client.GetAnimalsAsync(new AnimalGet { Id = id }, await Storage.GetMetadata());
        var animals = new List<AnimalDto>();
        foreach (var animal in reply.Animals)
        {
            animals.Add(new AnimalDto()
            {
                Id = animal.Id,
                Name = animal.Name,
                Type = Enum.TryParse<AnimalType>(animal.Type, true, out var parsedType) ? parsedType : AnimalType.Other,
                Photo = animal.Photo.ToByteArray(),
                Description = animal.Description
            });
        }
        return animals[0];
    }

    public async Task<string> CreateAnimal(AnimalDto animalDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        AnimalCreate animal = new()
        {
            Name = animalDto.Name,
            Type = animalDto.Type.ToString().ToLower(),
            Photo = ByteString.CopyFrom(animalDto.Photo),
            Description = animalDto.Description
        };
        var reply = await client.CreateAnimalAsync(animal, await Storage.GetMetadata());
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
            Type = animalDto.Type.ToString().ToLower(),
            Photo = ByteString.CopyFrom(animalDto.Photo),
            Description = animalDto.Description
        };
        var reply = await client.UpdateAnimalAsync(animal, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAnimal(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnimalService.AnimalServiceClient(channel);
        var reply = await client.DeleteAnimalAsync(new AnimalMinimal { Id = id }, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }
    #endregion

    #region Announcement
    public async Task<List<AnnouncementDto>> GetUserAnnouncements(StatusType? status = null)
    {
        var announcements = new List<AnnouncementDto>();
        var announcementParams = new AnnouncementGet()
        {
            AuthorId = await Storage.GetUserId()
        };
        if (status != null) announcementParams.Status = status.ToString()!.ToLower();
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnnouncementService.AnnouncementServiceClient(channel);
        var reply = await client.GetAnnouncementsAsync(announcementParams, await Storage.GetMetadata());
        foreach (var announcement in reply.Announcements)
        {
            announcements.Add(new AnnouncementDto()
            {
                Id = announcement.Id,
                AnimalId = announcement.AnimalId,
                Profit = announcement.KeeperProfit,
                IsNegotiable = announcement.IsNegotiable,
                Description = announcement.Description,
                StartTerm = announcement.StartTerm,
                EndTerm = announcement.EndTerm,
                Status = Enum.TryParse<StatusType>(announcement.Status, true, out var parsedStatus) ? parsedStatus : StatusType.Canceled,
                AddressId = announcement.AddressId,
                OwnerId = announcement.AuthorId,
                KeeperId = announcement.KeeperId
            });
        }
        return announcements;
    }

    public async Task<List<AnnouncementDto>> GetAnnouncements(int? minValue = null, int? maxValue = null, 
        DateTime? startTerm = null, DateTime? endTerm = null, string? keeperId = null, StatusType? status = null)
    {
        var announcements = new List<AnnouncementDto>();
        var announcementParams = new AnnouncementGet();
        if (minValue != null) announcementParams.KeeperProfitLess = (uint)minValue;
        if (maxValue != null) announcementParams.KeeperProfitMore = (uint)maxValue;
        if (startTerm != null) announcementParams.StartTermAfter = (ulong)new DateTimeOffset((DateTime)startTerm).ToUnixTimeSeconds();
        if (endTerm != null) announcementParams.EndTermBefore = (ulong)new DateTimeOffset((DateTime)endTerm).ToUnixTimeSeconds();
        if (keeperId != null) announcementParams.KeeperId = keeperId;
        if (status != null) announcementParams.Status = status.ToString()!.ToLower();
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnnouncementService.AnnouncementServiceClient(channel);
        var reply = await client.GetAnnouncementsAsync(announcementParams, await Storage.GetMetadata());
        foreach (var announcement in reply.Announcements)
        {
            announcements.Add(new AnnouncementDto()
            {
                Id = announcement.Id,
                AnimalId = announcement.AnimalId,
                Profit = announcement.KeeperProfit,
                IsNegotiable = announcement.IsNegotiable,
                Description = announcement.Description,
                StartTerm = announcement.StartTerm,
                EndTerm = announcement.EndTerm,
                Status = Enum.TryParse<StatusType>(announcement.Status, true, out var parsedStatus) ? parsedStatus : StatusType.Canceled,
                AddressId = announcement.AddressId,
                OwnerId = announcement.AuthorId,
                KeeperId = announcement.KeeperId
            });
        }
        return announcements;
    }

    public async Task<string> CreateAnnouncement(AnnouncementDto announcementDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnnouncementService.AnnouncementServiceClient(channel);
        AnnouncementCreate announcement = new()
        {
            AnimalId = announcementDto.AnimalId,
            KeeperProfit = announcementDto.Profit,
            IsNegotiable = announcementDto.IsNegotiable,
            Description = announcementDto.Description,
            StartTerm = announcementDto.StartTerm,
            EndTerm = announcementDto.EndTerm,
            Status = announcementDto.Status.ToString().ToLower(),
            AddressId = announcementDto.AddressId
        };
        var reply = await client.CreateAnnouncementAsync(announcement, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateAnnouncement(AnnouncementDto announcementDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnnouncementService.AnnouncementServiceClient(channel);
        AnnouncementUpdate announcement = new()
        {
            Id = announcementDto.Id,
            AnimalId = announcementDto.AnimalId,
            KeeperProfit = announcementDto.Profit,
            IsNegotiable = announcementDto.IsNegotiable,
            Description = announcementDto.Description,
            StartTerm = announcementDto.StartTerm,
            EndTerm = announcementDto.EndTerm,
            Status = announcementDto.Status.ToString().ToLower(),
            AddressId = announcementDto.AddressId
        };
        var reply = await client.UpdateAnnouncementAsync(announcement, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateAnnouncementStatus(UpdateAnnouncementDto announcementUpdateDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnnouncementService.AnnouncementServiceClient(channel);
        AnnouncementUpdate announcement = new()
        {
            Id = announcementUpdateDto.Id,
            Status = announcementUpdateDto.Status.ToString().ToLower(),
        };
        if (announcementUpdateDto.KeeperId != null)
            announcement.KeeperId = announcementUpdateDto.KeeperId;
        var reply = await client.UpdateAnnouncementAsync(announcement, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteAnnouncement(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new AnnouncementService.AnnouncementServiceClient(channel);
        var reply = await client.DeleteAnnouncementAsync(new AnnouncementMinimal { Id = id }, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }
    #endregion

    #region Opinions
    public async Task<List<OpinionDto>> GetOpinions(string? authorId = null, string? keeperId = null)
    {
        var opinions = new List<OpinionDto>();
        var opinionParams = new OpinionGet();
        if (authorId != null) opinionParams.AuthorId = authorId;
        if (keeperId != null) opinionParams.KeeperId = keeperId;
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new OpinionService.OpinionServiceClient(channel);
        var reply = await client.GetOpinionsAsync(opinionParams, await Storage.GetMetadata());
        foreach (var opinion in reply.Opinions)
        {
            opinions.Add(new OpinionDto()
            {
                Id = opinion.Id,
                AuthorId = opinion.AuthorId,
                KeeperId = opinion.KeeperId,
                Description = opinion.Description,
                CreatedDate = opinion.CreatedDate,
                Rating = opinion.Rating,
                AnnouncementId = opinion.AnnouncementId
            });
        }
        return opinions;
    }

    public async Task<string> CreateOpinion(OpinionDto opinionDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new OpinionService.OpinionServiceClient(channel);
        OpinionCreate opinion = new()
        {
            AuthorId = opinionDto.AuthorId,
            KeeperId = opinionDto.KeeperId,
            Description = opinionDto.Description ?? string.Empty,
            CreatedDate = opinionDto.CreatedDate,
            Rating = opinionDto.Rating,
            AnnouncementId = opinionDto.AnnouncementId
        };
        var reply = await client.CreateOpinionAsync(opinion, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }

    public async Task<string> UpdateOpinion(OpinionDto opinionDto)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new OpinionService.OpinionServiceClient(channel);
        OpinionUpdate opinion = new()
        {
            Id = opinionDto.Id,
            AuthorId = opinionDto.AuthorId,
            KeeperId = opinionDto.KeeperId,
            Description = opinionDto.Description ?? string.Empty,
            CreatedDate = opinionDto.CreatedDate,
            Rating = opinionDto.Rating,
            AnnouncementId = opinionDto.AnnouncementId
        };
        var reply = await client.UpdateOpinionAsync(opinion, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }

    public async Task<string> DeleteOpinion(string id)
    {
        using var channel = GrpcChannel.ForAddress($"http://{host}:{port}");
        var client = new OpinionService.OpinionServiceClient(channel);
        var reply = await client.DeleteOpinionAsync(new OpinionMinimal { Id = id }, await Storage.GetMetadata());
        return Wordings.SUCCESS;
    }
    #endregion
}

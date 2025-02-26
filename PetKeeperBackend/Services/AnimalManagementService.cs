﻿using Grpc.Core;
using Google.Protobuf;
using grpc_hello_world.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace grpc_hello_world.Services
{
    public class AnimalManagementService : AnimalService.AnimalServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnimalManagementService> _logger;

        public AnimalManagementService(AppDbContext context, ILogger<AnimalManagementService> logger)
        {
            _context = context;
            _logger = logger;
            Directory.CreateDirectory(UserManagementService._fileDir);
        }

        [Authorize]
        public override async Task<AnimalMinimal> CreateAnimal(AnimalCreate request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;
            bool isBanned = await _context.Users
                .Where(u => u.Id.ToString() == ownerId)
                .Select(u => u.IsBanned) // Only fetch IsBanned column
                .FirstOrDefaultAsync();
            if (isBanned)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User banned!"));
            }


            if (request.HasOwnerId && userRole == "Admin")
                ownerId = request.OwnerId;
            var animalId = Guid.NewGuid();

            byte[] photoBytes = request.Photo.ToByteArray();
            if (!UserManagementService.IsPngOrJpg(photoBytes))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Supplied photo is not PNG or JPG"));

            var userFileDir = Path.Combine(UserManagementService._fileDir, ownerId.ToString());
            var photoFilePath = Path.Combine(userFileDir, animalId.ToString(), "photo");
            Directory.CreateDirectory(Path.Combine(userFileDir, animalId.ToString()));

            var animal = new Animal
            {
                Id = animalId,
                Name = request.Name,
                OwnerId = Guid.Parse(ownerId),
                Type = request.Type,
                Description = request.Description,
                Photo = photoFilePath
            };

            try
            {
                _context.Animals.Add(animal);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }

            await File.WriteAllBytesAsync(photoFilePath, photoBytes);

            return new AnimalMinimal { Id = animal.Id.ToString(), OwnerId = ownerId };
        }

        [Authorize]
        public override async Task<AnimalList> GetAnimals(AnimalGet request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;
            bool isBanned = await _context.Users
                .Where(u => u.Id.ToString() == userId)
                .Select(u => u.IsBanned) // Only fetch IsBanned column
                .FirstOrDefaultAsync();
            if (isBanned)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User banned!"));
            }

            var query = _context.Animals.AsQueryable();

            if (request.HasId)
                query = query.Where(a => a.Id.ToString() == request.Id);

            if (request.HasName)
                query = query.Where(a => a.Name == request.Name);

            if (request.HasType)
                query = query.Where(a => a.Type == request.Type);

            if (request.HasDescription)
            {
                // Checks word by word in supplied description, and searches for descs containing the words
                var words = request.Description.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(a => words.Any(word => a.Description.Contains(word)));
            }

            if (request.HasOwnerId)
                if (string.IsNullOrEmpty(request.OwnerId))
                    query = query.Where(a => a.OwnerId.ToString() == userId);
                else
                    query = query.Where(a => a.OwnerId.ToString() == request.OwnerId);

            var animals = await query.ToListAsync();

            var animalList = new AnimalList();
            foreach (var a in animals)
            {
                var photoBytes = await File.ReadAllBytesAsync(a.Photo);
                var photoByteString = ByteString.CopyFrom(photoBytes);

                animalList.Animals.Add(new AnimalCreate
                {
                    Id = a.Id.ToString(),
                    OwnerId = a.OwnerId.ToString(),
                    Name = a.Name,
                    Photo = photoByteString,
                    Type = a.Type,
                    Description = a.Description ?? ""
                });
            }

            return animalList;

        }

        [Authorize]
        public override async Task<AnimalMinimal> DeleteAnimal(AnimalMinimal request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;
            bool isBanned = await _context.Users
                .Where(u => u.Id.ToString() == ownerId)
                .Select(u => u.IsBanned) // Only fetch IsBanned column
                .FirstOrDefaultAsync();
            if (isBanned)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User banned!"));
            }


            try
            {
                Animal? animal;
                if (userRole == "Admin")
                {
                    animal = await _context.Animals.SingleOrDefaultAsync(
                    a => a.Id.ToString() == request.Id);
                }
                else
                {
                    animal = await _context.Animals.SingleOrDefaultAsync(
                    a => a.Id.ToString() == request.Id && a.OwnerId.ToString() == ownerId);
                }

                if (animal == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Animal does not exist"));

                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();

                return new AnimalMinimal { Id = animal.Id.ToString(), OwnerId = animal.OwnerId.ToString() };
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }
        }

        [Authorize]
        public override async Task<AnimalUpdate> UpdateAnimal(AnimalUpdate request, ServerCallContext context)
        {
            var request_type = request.GetType();

            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;
            bool isBanned = await _context.Users
                .Where(u => u.Id.ToString() == ownerId)
                .Select(u => u.IsBanned) // Only fetch IsBanned column
                .FirstOrDefaultAsync();
            if (isBanned)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User banned!"));
            }


            Animal? animal;
            if (userRole == "Admin")
            {
                animal = await _context.Animals.FirstOrDefaultAsync(
                    a => a.Id.ToString() == request.Id);               
            }
            else
            {
                animal = await _context.Animals.FirstOrDefaultAsync(
                    a => a.Id.ToString() == request.Id && a.OwnerId.ToString() == ownerId);
            }
            if (animal == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Animal not found!"));
            }
            

            var requestProperties = request_type.GetProperties();
            var modifiedProperties = new HashSet<string>();

            foreach (var property in requestProperties)
            {
                var hasProperty = request_type.GetProperty($"Has{property.Name}")?.GetValue(request, null) as bool?;
                if (hasProperty != true) // Skip if object is not present or avilability information is unavailable (null)
                    continue;

                var newValue = request_type.GetProperty(property.Name)?.GetValue(request, null);
                if (newValue == null)  // Skip if value is not available in the request (this guard might be optional, since this should not occur)
                    continue;

                var currentValue = typeof(Animal).GetProperty(property.Name)?.GetValue(animal, null);
                bool condition = !Equals(currentValue, newValue);
                if (property.Name == "Photo")
                {
                    var userFileDir = Path.Combine(UserManagementService._fileDir, animal.OwnerId.ToString());
                    var photoFilePath = Path.Combine(userFileDir, animal.Id.ToString(), "photo");
                    byte[] currentValueBytes = UserManagementService.PngSignature;
                    try
                    {
                        currentValueBytes = await File.ReadAllBytesAsync(photoFilePath);
                    }
                    catch (FileNotFoundException)
                    {
                        throw new RpcException(new Status(StatusCode.DataLoss, "Animals is missing a photo"));
                    }
                    byte[] newValueBytes = request.Photo.ToByteArray();
                    if (!UserManagementService.IsPngOrJpg(newValueBytes))
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Supplied photo is not a PNG or JPG"));

                    condition = !currentValueBytes.SequenceEqual(newValueBytes);
                    if (condition)
                    {
                        await File.WriteAllBytesAsync(photoFilePath, newValueBytes);
                    }
                }
                if (condition)
                {
                    if (property.Name != "Photo")  // Don't set new value since filepath doesn't change
                        typeof(Animal).GetProperty(property.Name)?.SetValue(animal, newValue);
                    modifiedProperties.Add(property.Name);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }

            var response = new AnimalUpdate
            {
                Id = animal.Id.ToString()
            };

            // Include only updated fields in the response
            foreach (var propertyName in modifiedProperties)
            {
                var property = request_type.GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(Animal).GetProperty(propertyName)?.GetValue(animal, null);
                    if (propertyName == "Photo")
                        value = ByteString.CopyFrom(new byte[] { 0x01 });  // Send just a single byte to indicate change
                    property.SetValue(response, value);
                }
            }

            return response;
        }
    }
    
}

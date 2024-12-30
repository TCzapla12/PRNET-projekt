using Grpc.Core;
using grpc_hello_world;
using grpc_hello_world.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Drawing.Printing;
using System.Reflection;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Reflection.Metadata.Ecma335;
using Azure.Core;

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
        }

        [Authorize]
        public override async Task<AnimalMinimal> CreateAnimal(AnimalCreate request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var animal = new Animal
            {
                Name = request.Name,
                OwnerId = Guid.Parse(ownerId),
                Type = request.Type,
                Description = request.Description ?? "",
                Photos = request.Photos.ToArray()
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

            return new AnimalMinimal { Id = animal.Id.ToString(), OwnerId = ownerId };
        }

        [Authorize]
        public override async Task<AnimalList> GetAnimals(AnimalGet request, ServerCallContext context)
        {
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

            var animals = await query.ToListAsync();

            var addressList = new AnimalList();
            addressList.Animals.AddRange(animals.Select(a => new AnimalCreate
            {
                Id = a.Id.ToString(),
                OwnerId = a.OwnerId.ToString(),
                Name = a.Name,
                Type = a.Type,
                Description = a.Description ?? ""
            }));

            return addressList;

        }

        [Authorize]
        public override async Task<AnimalMinimal> DeleteAnimal(AnimalMinimal request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                Animal? animal = await _context.Animals.SingleOrDefaultAsync(
                    a => a.Id.ToString() == request.Id && a.OwnerId.ToString() == ownerId)
                    ?? throw new RpcException(new Status(StatusCode.NotFound, "Animal does not exist"));

                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();

                return new AnimalMinimal { Id = animal.Id.ToString(), OwnerId = animal.OwnerId.ToString()  };
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
            Animal? animal = await _context.Animals.FirstOrDefaultAsync(
                a => a.Id.ToString() == request.Id && a.OwnerId.ToString() == ownerId)
                ??
                throw new RpcException(new Status(StatusCode.NotFound, "Animal not found!"));

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

                if (!Equals(currentValue, newValue))
                {
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
                    property.SetValue(response, value);
                }
            }

            return response;
        }
    }
    
}

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
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace grpc_hello_world.Services
{
    public class AddressManagementService : AddressService.AddressServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AddressManagementService> _logger;

        public AddressManagementService(AppDbContext context, ILogger<AddressManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public override async Task<AddressMinimal> CreateAddress(AddressCreate request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var address = new Address
            {
                Street = request.Street,
                HouseNumber = request.HouseNumber,
                ApartmentNumber = request.ApartmentNumber ?? null,
                City = request.City,
                PostCode = request.PostCode,
                Description = request.Description,
                OwnerId = Guid.Parse(userId),
                IsPrimary = false  // Primary addresses are created during account creation only!
            };

            try
            {
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }

            return new AddressMinimal
            {
                Id = address.Id.ToString(),
                OwnerId = address.OwnerId.ToString()
            };
        }

        [Authorize]
        public override async Task<AddressList> GetUserAddresses(AddressGet request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // For now only id and owner_id supported
            List<Address> addresses = await _context.Addresses
                .Where(a => a.OwnerId == Guid.Parse(ownerId))
                .ToListAsync();

            var ret = new AddressList();
            ret.Addresses.AddRange(addresses.Select(a => new AddressCreate
            {
                Id = a.Id.ToString(),
                Street = a.Street,
                HouseNumber = a.HouseNumber,
                ApartmentNumber = a.ApartmentNumber ?? "",
                City = a.City,
                PostCode = a.PostCode,
                // OwnerId = a.OwnerId.ToString(),
                Description = a.Description ?? ""
            }));
            return ret;
        }

        public override async Task<AddressMinimal> DeleteAddress(AddressMinimal request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                Address? address = await _context.Addresses.SingleOrDefaultAsync(a => a.Id.ToString() == request.Id);
                if (address == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound,
                        "Address does not exist"));
                }
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                return new AddressMinimal { Id = address.Id.ToString() };
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }
        }

        public override async Task<AddressList> GetAddress(AddressGet request, ServerCallContext context)
        {
            var query = _context.Addresses.AsQueryable();

            if (request.HasId)
                query = query.Where(a => a.Id.ToString() == request.Id);

            if (request.HasStreet)
                query = query.Where(a => a.Street == request.Street);

            if (request.HasHouseNumber)
                query = query.Where(a => a.HouseNumber == request.HouseNumber);

            if (request.HasCity)
                query = query.Where(a => a.City == request.City);

            if (request.HasPostCode)
                query = query.Where(a => a.PostCode == request.PostCode);

            if (request.HasIsPrimary)
                query = query.Where(a => a.IsPrimary == request.IsPrimary);

            if (request.HasOwnerId)
            {
                if (Guid.TryParse(request.OwnerId, out var ownerIdGuid))
                    query = query.Where(a => a.OwnerId == ownerIdGuid);
                else
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "GUID format invalid"));
            }

            var addresses = await query.ToListAsync();

            var addressList = new AddressList();
            addressList.Addresses.AddRange(addresses.Select(a => new AddressCreate
            {
                Id = a.Id.ToString(),
                Street = a.Street,
                HouseNumber = a.HouseNumber,
                City = a.City,
                IsPrimary = a.IsPrimary,
                PostCode = a.PostCode,
                OwnerId = a.OwnerId.ToString()
            }));

            return addressList;
        }


        public override async Task<AddressUpdate> UpdateAddress(AddressUpdate request, ServerCallContext context)
        {
            var request_type = request.GetType();

            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Address? address = await _context.Addresses.FirstOrDefaultAsync(
                a => a.Id.ToString() == request.Id && a.OwnerId.ToString() == ownerId)
                ??
                throw new RpcException(new Status(StatusCode.NotFound, "Address not found!"));

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

                var currentValue = typeof(Address).GetProperty(property.Name)?.GetValue(address, null);

                if (!Equals(currentValue, newValue))
                {
                    typeof(Address).GetProperty(property.Name)?.SetValue(address, newValue);
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

            var response = new AddressUpdate
            {
                Id = address.Id.ToString()
            };

            // Include only updated fields in the response
            foreach (var propertyName in modifiedProperties)
            {
                var property = request_type.GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(Address).GetProperty(propertyName)?.GetValue(address, null);
                    property.SetValue(response, value);
                }
            }

            return response;
        }
    }
}

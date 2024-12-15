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
    }
}

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

namespace grpc_hello_world.Services
{
    public class UserManagementService : UserService.UserServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(AppDbContext context, ILogger<UserManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<UserMinimal> CreateUser(UserCreate request, ServerCallContext context)
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password), // PKCS2 hashed password
                FirstName = request.FirstName,
                LastName = request.LastName,
                // Advanced data
                Phone = request.Phone,
                Pesel = request.Pesel,
                AvatarUrl = request.AvatarUrl,
                DocumentUrl = request.DocumentUrl,
                // States
                IsActivated = false,
                IsVerified = false,
                IsBanned = false,
                IsAdmin = false
            };
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }

            var address = new Address
            {
                City = request.PrimaryAddress.City,
                Street = request.PrimaryAddress.Street,  // Theoretically verification if the number is here is required
                HouseNumber = request.PrimaryAddress.HouseNumber ?? null,
                PostCode = request.PrimaryAddress.PostCode,
                OwnerEmail = request.Email,
                IsPrimary = true
            };

            try
            {
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e)
            {
                _context.Users.Remove(user);
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }
            

            return new UserMinimal
            {
                Email = user.Email
            };
        }

        [Authorize]
        public override async Task<UserFull> GetUser(UserGet request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userEmail = userContext.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _context.Users.FindAsync(userEmail);
                //.Include(u => u.Address) // Eager
                //.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            var primary_address = await _context.Addresses.
                FirstOrDefaultAsync(a => a.OwnerEmail == userEmail && a.IsPrimary);
            if (primary_address == null)
            {
                throw new RpcException(new Status(StatusCode.DataLoss, "User has no primary address!"));
            }
            return new UserFull
            {
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                // Advanced data
                Phone = user.Phone,
                Pesel = user.Pesel,
                PrimaryAddress = new AddressCreate
                {
                    Id = primary_address.Id.ToString(),
                    Street = primary_address.Street,
                    HouseNumber = primary_address.HouseNumber ?? null,
                    City = primary_address.City,
                    PostCode = primary_address.PostCode,
                    OwnerEmail = primary_address.OwnerEmail
                },
                AvatarUrl = user.AvatarUrl,
                DocumentUrl = user.DocumentUrl,
                // Flags
                IsActivated = user.IsActivated,
                IsVerified = user.IsVerified,
                IsBanned = user.IsBanned,
                IsAdmin = user.IsAdmin
            };
        }

        public override async Task<UserUpdate> UpdateUser(UserUpdate request, ServerCallContext context)
        {
            // Find the user in the database
            var user = await _context.Users.FindAsync(request.Email) 
                       ?? throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            var requestProperties = typeof(UserFull).GetProperties();
            var modifiedProperties = new HashSet<string>();

            foreach (var property in requestProperties)
            {
                // Check if the property has a corresponding "Has<PropertyName>" indicator
                var hasProperty = typeof(UserFull).GetProperty($"Has{property.Name}")?.GetValue(request, null) as bool?;
                if (hasProperty != true) // Skip if presence information is false or unavailable (null)
                    continue;

                // Get the new value from the request
                var newValue = typeof(UserFull).GetProperty(property.Name)?.GetValue(request, null);
                if (newValue == null) // Skip if the value is null
                    continue;

                // Get the current value from the database object
                var currentValue = typeof(User).GetProperty(property.Name)?.GetValue(user, null);

                // Only update if the new value is different from the current value
                if (!Equals(currentValue, newValue))
                {
                    typeof(User).GetProperty(property.Name)?.SetValue(user, newValue);
                    modifiedProperties.Add(property.Name); // Track the property name
                }
            }

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }

            // Build the UserFull response with modified fields only
            var UserResponse = new UserUpdate { Email = user.Email};

            foreach (var propertyName in modifiedProperties)
            {
                var property = typeof(UserFull).GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(User).GetProperty(propertyName)?.GetValue(user, null);
                    property.SetValue(UserResponse, value);
                }
            }

            return UserResponse;

        }


        public override async Task<UserMinimal> DeleteUser(UserMinimal request, ServerCallContext context)
        {
            var user = await _context.Users.FindAsync(request.Email);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new UserMinimal { Email = request.Email };
        }

        public static string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.HashPassword(null, password); // null is the user object; it's not needed for this case
        }
        public static bool VerifyPassword(string password, string hash)
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(null, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}

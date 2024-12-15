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
    public class UserManagementService : UserService.UserServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(AppDbContext context, ILogger<UserManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<UserIdentifier> CreateUser(UserCreate request, ServerCallContext context)
        {
            if (request.DocumentUrl.Count != 2)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "Minimum two document pictures required!"));
            }
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
                DocumentUrl = request.DocumentUrl.ToArray(),
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
                Street = request.PrimaryAddress.Street,  // TODO: verification if the number is here is required
                HouseNumber = request.PrimaryAddress.HouseNumber,
                ApartmentNumber = request.PrimaryAddress.ApartmentNumber ?? null,
                PostCode = request.PrimaryAddress.PostCode,
                OwnerId = user.Id,
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
            

            return new UserIdentifier
            {
                Id = user.Id.ToString()
            };
        }

        [Authorize]
        public override async Task<UserFull> GetUser(UserGet request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userEmail = userContext.FindFirst(ClaimTypes.Email)?.Value;
            if (!(request.UserId.HasUsername || request.UserId.HasEmail || request.UserId.HasId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "User identificaiton not provided. Please provide ID, email or username"));
            }
            User? user = await GetUserAsync(userEmail, request.UserId, _context);
            
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    "User not found"));
            }

            Address? primary_address = await _context.Addresses.
                FirstOrDefaultAsync(a => a.OwnerId == user.Id && a.IsPrimary);
            if (primary_address == null)
            {
                throw new RpcException(new Status(StatusCode.DataLoss,
                    "User has no primary address!"));
            }

            var ret = new UserFull
            {
                Id = user.Id.ToString(),
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
                    HouseNumber = primary_address.HouseNumber,
                    ApartmentNumber = primary_address.ApartmentNumber ?? null,
                    City = primary_address.City,
                    PostCode = primary_address.PostCode,
                    OwnerId = primary_address.OwnerId.ToString()
                },
                AvatarUrl = user.AvatarUrl,
                // Flags
                IsActivated = user.IsActivated,
                IsVerified = user.IsVerified,
                IsBanned = user.IsBanned,
                IsAdmin = user.IsAdmin          
            };
            ret.DocumentUrl.Add(user.DocumentUrl[0]);
            ret.DocumentUrl.Add(user.DocumentUrl[1]);
            return ret;

        }

        [Authorize]
        public override async Task<UserUpdate> UpdateUser(UserUpdate request, ServerCallContext context)
        {
            var request_type = request.GetType();

            var userContext = context.GetHttpContext().User;
            var userEmail = userContext.FindFirst(ClaimTypes.Email)?.Value;
            User? user = await GetUserAsync(userEmail, request.UserId, _context);

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

                var currentValue = typeof(User).GetProperty(property.Name)?.GetValue(user, null);

                if (!Equals(currentValue, newValue))
                {
                    typeof(User).GetProperty(property.Name)?.SetValue(user, newValue);
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

            // Build the UserFull response with modified fields only
            var UserResponse = new UserUpdate
            {
                UserId = new UserIdentifier { Email = user.Email }
            };

            foreach (var propertyName in modifiedProperties)
            {
                var property = request_type.GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(User).GetProperty(propertyName)?.GetValue(user, null);
                    property.SetValue(UserResponse, value);
                }
            }

            return UserResponse;

        }

        [Authorize]
        public override async Task<UserIdentifier> DeleteUser(UserIdentifier request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userEmail = userContext.FindFirst(ClaimTypes.Email)?.Value;
            User user = await GetUserAsync(userEmail, request, _context);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new UserIdentifier { Email = user.Email };
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

        public static async Task<User> GetUserAsync(string context_email, UserIdentifier user_identifier, AppDbContext context)
        {
            User? user = null;
            switch (user_identifier.KeyCase)
            {
                case UserIdentifier.KeyOneofCase.Email:
                    // TODO: Add actual admin checking, with ClaimTypes.Role
                    if (context_email != null && user_identifier.Email != context_email && context_email != "admin@admin.com" )
                    {
                        throw new RpcException(new Status(StatusCode.Unauthenticated,
                            "You need to be an admin to read other users data!"));
                    }
                    user = await context.Users.FirstOrDefaultAsync(u => u.Email == user_identifier.Email);
                    break;

                case UserIdentifier.KeyOneofCase.Id:
                    user = await context.Users.FindAsync(user_identifier.Id);
                    break;

                case UserIdentifier.KeyOneofCase.Username:
                    user = await context.Users.FirstOrDefaultAsync(u => u.Username == user_identifier.Username);
                    break;

                case UserIdentifier.KeyOneofCase.None:
                    user = await context.Users.FirstOrDefaultAsync(u => u.Email == context_email);
                    break;
            }
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    "User not found"));
            }
            return user;
        }
    }
}

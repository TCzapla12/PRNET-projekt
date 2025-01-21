using Grpc.Core;
using Google.Protobuf;
using grpc_hello_world.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace grpc_hello_world.Services
{
    public class UserManagementService : UserService.UserServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserManagementService> _logger;
        public static readonly string _fileDir = "/home/app/uploads/";
        public static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        public static readonly byte[] JpgSignature = [ 0xFF, 0xD8, 0xFF ];


        public UserManagementService(AppDbContext context, ILogger<UserManagementService> logger)
        {
            _context = context;
            _logger = logger;
            Directory.CreateDirectory(_fileDir);
        }

        public override async Task<UserIdentifier> CreateUser(UserCreate request, ServerCallContext context)
        {
            if (request.DocumentPngs.Count != 2)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "Minimum two document pictures required!"));
            }
            var userId = Guid.NewGuid();

            var userFileDir = Path.Combine(_fileDir, userId.ToString());
            Directory.CreateDirectory(userFileDir);
            string? avatarFilePath = null;
            if (request.HasAvatarPng)
            {
                avatarFilePath = Path.Combine(userFileDir, "avatar");
            }
            
            string[] documentFilePath = [
                Path.Combine(userFileDir, "documentFront"),
                Path.Combine(userFileDir, "documentBack")
            ];

            if (!IsPngOrJpg(request.DocumentPngs[0].ToByteArray()) ||
                !IsPngOrJpg(request.DocumentPngs[1].ToByteArray()) ||
                request.HasAvatarPng && !IsPngOrJpg(request.AvatarPng.ToByteArray()))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "One or more of supplied images are not PNG or JPG!"));
            }

            var user = new User
            {
                Id = userId,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password), // PKCS2 hashed password
                FirstName = request.FirstName,
                LastName = request.LastName,
                // Advanced data
                Phone = request.Phone,
                Pesel = request.Pesel,
                AvatarUrl = avatarFilePath,
                DocumentUrl = documentFilePath,
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

            await File.WriteAllBytesAsync(documentFilePath[0], request.DocumentPngs[0].ToByteArray());
            await File.WriteAllBytesAsync(documentFilePath[1], request.DocumentPngs[1].ToByteArray());
            if (request.HasAvatarPng && avatarFilePath != null)
                await File.WriteAllBytesAsync(avatarFilePath, request.AvatarPng.ToByteArray());

            var address = new Address
            {
                City = request.PrimaryAddress.City,
                Street = request.PrimaryAddress.Street,  // TODO: verification if the number is here is required
                HouseNumber = request.PrimaryAddress.HouseNumber,
                ApartmentNumber = string.IsNullOrEmpty(request.PrimaryAddress.ApartmentNumber)
                ? null
                : request.PrimaryAddress.ApartmentNumber,
                PostCode = request.PrimaryAddress.PostCode,
                OwnerId = user.Id,
                Description = string.IsNullOrEmpty(request.PrimaryAddress.Description)
                ? null
                : request.PrimaryAddress.Description,
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
                Id = user.Id.ToString(),
                PrimaryAddressId = address.Id.ToString()
            };
        }

        [Authorize]
        public override async Task<UserFull> GetUser(UserGet request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            /* admin-like call to allow requesting other user's data. The result is truncated of sensitive data later on */
            User? user = await GetUserAsync(request.UserId, context, _context, true);

            Address? primary_address = await _context.Addresses.FirstOrDefaultAsync(
                a => a.OwnerId == user.Id && a.IsPrimary);
            if (primary_address == null)
            {
                throw new RpcException(new Status(StatusCode.DataLoss, "User has no primary address!"));
            }
            /* Full data can be given only if user is an admin, or if user is requesting it's own data */
            bool emailNotSetOrEqual = !request.UserId.HasEmail || user.Email == request.UserId.Email;
            bool idNotSetOrEqual = !request.UserId.HasId || user.Id.ToString() == request.UserId.Id;
            bool usernameNotSetOrEqual = !request.UserId.HasUsername || user.Username == request.UserId.Username;
            bool isAdminOrSelf = userRole == "Admin" || (emailNotSetOrEqual && idNotSetOrEqual && usernameNotSetOrEqual);

            var userFileDir = Path.Combine(_fileDir, user.Id.ToString());

            byte[]? avatarBytes = null;
            try
            {                
                if (user.AvatarUrl != null)
                {
                    var avatarFilePath = Path.Combine(userFileDir, "avatar");
                    avatarBytes = await File.ReadAllBytesAsync(avatarFilePath);
                }
                
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Error reading avatar file"));
            }
            byte[][] documentBytes = new byte[2][];
            try
            {
                var documentFrontFilePath = Path.Combine(userFileDir, "documentFront");
                var documentBackFilePath = Path.Combine(userFileDir, "documentBack");
                documentBytes[0] = await File.ReadAllBytesAsync(documentFrontFilePath);
                documentBytes[1] = await File.ReadAllBytesAsync(documentBackFilePath);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Error reading document file(s)"));
            }
            /* Protected user data - PESEL, primary Address, document URLs are truncated if requester is not an Admin */
            var ret = new UserFull
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                // Advanced data
                Phone = user.Phone,
                Pesel = isAdminOrSelf ? user.Pesel : "", // Truncate: sensitive 
                PrimaryAddress = new AddressCreate
                {
                    Id = primary_address.Id.ToString(),
                    Street = primary_address.Street,
                    HouseNumber = primary_address.HouseNumber,
                    ApartmentNumber = primary_address.ApartmentNumber ?? "",
                    City = primary_address.City,
                    PostCode = primary_address.PostCode,
                    OwnerId = primary_address.OwnerId.ToString()
                },
                // Flags
                IsActivated = user.IsActivated,
                IsVerified = user.IsVerified,
                IsBanned = user.IsBanned,
                IsAdmin = user.IsAdmin          
            };
            if (avatarBytes != null)
                ret.AvatarPng = ByteString.CopyFrom(avatarBytes);
            if (isAdminOrSelf) // Truncate: sensitive
            {
                ret.DocumentPngs.Add(ByteString.CopyFrom(documentBytes[0]));
                ret.DocumentPngs.Add(ByteString.CopyFrom(documentBytes[1]));
            }

            return ret;

        }

        [Authorize]
        public override async Task<UserUpdate> UpdateUser(UserUpdate request, ServerCallContext context)
        {
            var request_type = request.GetType();
            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            User? user = await GetUserAsync(request.UserId, context, _context);

            var requestProperties = request_type.GetProperties();
            var modifiedProperties = new HashSet<string>();

            string[] adminOnlyProperties = ["IsActivated", "IsAdmin", "IsBanned", "IsVerified"];
            foreach (var property in requestProperties)
            {
                if (adminOnlyProperties.Contains(property.Name) && userRole != "Admin")
                    continue;

                var hasProperty = request_type.GetProperty($"Has{property.Name}")?.GetValue(request, null) as bool?;
                if (hasProperty != true) // Skip if object is not present or avilability information is unavailable (null)
                    continue;

                var newValue = request_type.GetProperty(property.Name)?.GetValue(request, null);
                if (newValue == null)  // Skip if value is not available in the request (this guard might be optional, since this should not occur)
                    continue;

                var currentValue = typeof(User).GetProperty(property.Name)?.GetValue(user, null);

                bool condition = !Equals(currentValue, newValue);
                if (property.Name == "AvatarPng")
                {
                    var userFileDir = Path.Combine(_fileDir, user.Id.ToString());
                    var avatarFilePath = Path.Combine(userFileDir, "avatar");
                    byte[] currentValueBytes = PngSignature;
                    try
                    {
                        currentValueBytes = await File.ReadAllBytesAsync(avatarFilePath);
                    }
                    catch (FileNotFoundException)
                    {
                        typeof(User).GetProperty("AvatarUrl")?.SetValue(user, avatarFilePath);
                    }
                    
                    byte[] newValueBytes = request.AvatarPng.ToByteArray();
                    if (!IsPngOrJpg(newValueBytes))
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Supplied avatar is not a PNG or JPG"));

                    condition = !currentValueBytes.SequenceEqual(newValueBytes);
                    if (condition)
                    {
                        await File.WriteAllBytesAsync(avatarFilePath, newValueBytes);
                    }
                }
                if (property.Name == "DocumentPngs")
                {
                    var userFileDir = Path.Combine(_fileDir, user.Id.ToString());

                    var documentFrontFilePath = Path.Combine(userFileDir, "documentFront");
                    var documentBackFilePath = Path.Combine(userFileDir, "documentBack");
                    byte[] currentDocumentFront = PngSignature;
                    byte[] currentDocumentBack = PngSignature;
                    try
                    {
                        currentDocumentFront = await File.ReadAllBytesAsync(documentFrontFilePath);
                    }
                    catch (FileNotFoundException)
                    {
                        throw new RpcException(new Status(StatusCode.DataLoss, "User is missing a front document photo"));
                    }
                    try
                    {
                        currentDocumentBack = await File.ReadAllBytesAsync(documentBackFilePath);
                    }
                    catch (FileNotFoundException)
                    {
                        throw new RpcException(new Status(StatusCode.DataLoss, "User is missing a back document photo"));
                    }

                    var newDocumentFront = request.DocumentPngs[0].ToByteArray();
                    var newDocumentBack = request.DocumentPngs[1].ToByteArray();
                    if (!IsPngOrJpg(newDocumentBack) || !IsPngOrJpg(newDocumentFront))
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Supplied document picture(s) are not a PNG or JPG"));

                    condition = !(currentDocumentFront.SequenceEqual(newDocumentFront) && currentDocumentBack.SequenceEqual(newDocumentBack));
                    if (condition)
                    {
                        await File.WriteAllBytesAsync(documentFrontFilePath, newDocumentFront);
                        await File.WriteAllBytesAsync(documentBackFilePath, newDocumentBack);
                    }                 
                }

                if (condition)
                {
                    if (!property.Name.Contains("Png"))  // Don't set new value since filepath doesn't change
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

            var UserResponse = new UserUpdate
            {
                UserId = new UserIdentifier { Id = user.Id.ToString() }
            };

            foreach (var propertyName in modifiedProperties)
            {
                var property = request_type.GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(User).GetProperty(propertyName)?.GetValue(user, null);
                    if (propertyName.Contains("Png"))
                        value = ByteString.CopyFrom(new byte[] { 0x01 });  // Send just a single byte to indicate change
                    property.SetValue(UserResponse, value);
                }
            }

            return UserResponse;

        }
        [Authorize]
        public override async Task<UserUpdate> UpdateUserCredentials(UserUpdateCredentials request, ServerCallContext context)
        {
            /* request.PasswordHash is a raw password. The naming is like this to make reflection work */
            var request_type = request.GetType();

            User? user = await GetUserAsync(request.UserId, context, _context);

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
                if (property.Name == "PasswordHash")
                    newValue = HashPassword(request.PasswordHash);

                var currentValue = typeof(User).GetProperty(property.Name)?.GetValue(user, null);

                bool condition = !Equals(currentValue, newValue);
                if (property.Name == "PasswordHash")
                {
                    condition = VerifyPassword(request.PasswordHash, user.PasswordHash);

                }
                if (condition)
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

            var response = new UserUpdate
            {
                UserId = new UserIdentifier { Id = user.Id.ToString() }
            };

            // Include only updated fields in the response
            foreach (var propertyName in modifiedProperties)
            {
                var property = typeof(UserUpdate).GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(User).GetProperty(propertyName)?.GetValue(user, null);
                    if (propertyName == "PasswordHash")
                        value = true;

                    property.SetValue(response, value);
                }
            }

            return response;

        }
        [Authorize]
        public override async Task<UserIdentifier> DeleteUser(UserIdentifier request, ServerCallContext context)
        {

            User? user = await GetUserAsync(request, context, _context);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new UserIdentifier { Id = user.Id.ToString() };
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

        public static bool IsPngOrJpg(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                return false;

            if (fileBytes.Length >= PngSignature.Length)
            {
                bool isPng = true;
                for (int i = 0; i < PngSignature.Length; i++)
                {
                    if (fileBytes[i] != PngSignature[i])
                    {
                        isPng = false;
                        break;
                    }
                }
                if (isPng) return true;
            }

            if (fileBytes.Length >= JpgSignature.Length)
            {
                bool isJpg = true;
                for (int i = 0; i < JpgSignature.Length; i++)
                {
                    if (fileBytes[i] != JpgSignature[i])
                    {
                        isJpg = false;
                        break;
                    }
                }
                if (isJpg) return true;
            }
            return false;
        }

        public static async Task<User> GetUserAsync(
            UserIdentifier user_identifier,
            ServerCallContext callContext,
            AppDbContext dbContext,
            bool overrideFlag = false
        )
        {
            var userContext = callContext.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && !overrideFlag)
            {
                user_identifier = new UserIdentifier { Id = userId };
            }

            User? user = null;
            switch (user_identifier.KeyCase)
            {
                case UserIdentifier.KeyOneofCase.Email:
                    user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == user_identifier.Email);
                    break;

                case UserIdentifier.KeyOneofCase.Id:
                    user = await dbContext.Users.FindAsync(Guid.Parse(user_identifier.Id));
                    break;

                case UserIdentifier.KeyOneofCase.Username:
                    user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == user_identifier.Username);
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

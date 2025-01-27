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
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace grpc_hello_world.Services
{
    public class AnnouncementManagementService : AnnouncementService.AnnouncementServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnnouncementManagementService> _logger;

        public AnnouncementManagementService(AppDbContext context, ILogger<AnnouncementManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public override async Task<AnnouncementMinimal> CreateAnnouncement(AnnouncementCreate request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Admin")  // If user is Admin
            {
                if (request.HasAuthorId)
                    userId = request.AuthorId;
            }

            /* Even in case of Admin operation, Animal and Address should belong to the Announcement author */
            Animal? animal = await _context.Animals.SingleOrDefaultAsync(
                a => a.Id.ToString() == request.AnimalId && a.OwnerId == Guid.Parse(userId));
            if (animal == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    "Animal does not exist for this user"));
            }

            Address? address = await _context.Addresses.SingleOrDefaultAsync(
                a => a.Id.ToString() == request.AddressId && a.OwnerId == Guid.Parse(userId));
            if (address == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    "Address does not exist for this user."));
            }

            var announcement = new Announcement
            {
                AuthorId = Guid.Parse(userId),
                // KeeperId set in update request
                AnimalId = Guid.Parse(request.AnimalId),
                KeeperProfit = request.KeeperProfit,
                IsNegotiable = request.IsNegotiable,
                Description = request.Description,
                StartTerm = request.StartTerm,
                EndTerm = request.EndTerm,
                // Created date set in db. Started and Finished Date set in update request
                Status = "created",
                AddressId = Guid.Parse(request.AddressId)
            };
            try
            {
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }

            return new AnnouncementMinimal
            {
                Id = announcement.Id.ToString(),
                AuthorId = announcement.AuthorId.ToString(),
                KeeperId = "",
                AnimalId = announcement.AnimalId.ToString(),
                AddressId = announcement.AddressId.ToString()
            };
        }

        [Authorize]
        public override async Task<AnnouncementList> GetAnnouncements(AnnouncementGet request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var authorId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.Announcements.AsQueryable();

            if (request.HasId)
            {
                if (Guid.TryParse(request.Id, out var IdGuid))
                    query = query.Where(a => a.Id == IdGuid);
                else
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "GUID format invalid for Id"));
                query = query.Where(a => a.Id.ToString() == request.Id);
            }

            if (request.HasAuthorId)
            {
                query = query.Where(a => a.AuthorId.ToString() == request.AuthorId);
            }
                
            if (request.HasKeeperId)
            {
                if (Guid.TryParse(request.Id, out var KeeperIdGuid))
                    query = query.Where(a => a.Id == KeeperIdGuid);
                else
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "GUID format invalid for KeeperId"));
                query = query.Where(a => a.KeeperId.ToString() == request.KeeperId);
            }
                
            if (request.HasAnimalId)
            {
                if (Guid.TryParse(request.Id, out var KeeperIdGuid))
                    query = query.Where(a => a.Id == KeeperIdGuid);
                else
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "GUID format invalid for AnimalId"));
                query = query.Where(a => a.AnimalId.ToString() == request.AnimalId);
            }
                

            if (request.HasKeeperProfitMore)
                query = query.Where(a => a.KeeperProfit >= request.KeeperProfitMore);

            if (request.HasKeeperProfitLess)
                query = query.Where(a => a.KeeperProfit <= request.KeeperProfitLess);

            if (request.HasIsNegotiable)
                query = query.Where(a => a.IsNegotiable == request.IsNegotiable);

            if (request.HasDescription)
                query = query.Where(a => a.Description == request.Description);

            if (request.HasStartTermBefore)
                query = query.Where(a => a.StartTerm <= request.StartTermBefore);

            if (request.HasEndTermBefore)
                query = query.Where(a => a.EndTerm <= request.EndTermBefore);

            if (request.HasCreatedDateBefore)
                query = query.Where(a => a.CreatedDate <= request.CreatedDateBefore);

            if (request.HasFinishedDateBefore)
                query = query.Where(a => a.FinishedDate <= request.FinishedDateBefore);

            if (request.HasStartTermAfter)
                query = query.Where(a => a.StartTerm >= request.StartTermAfter);

            if (request.HasEndTermAfter)
                query = query.Where(a => a.EndTerm >= request.EndTermAfter);

            if (request.HasCreatedDateAfter)
                query = query.Where(a => a.CreatedDate >= request.CreatedDateAfter);

            if (request.HasFinishedDateAfter)
                query = query.Where(a => a.FinishedDate >= request.FinishedDateAfter);

            if (request.HasStatus)
                query = query.Where(a => a.Status == request.Status);

            if (request.HasAddressId)
                query = query.Where(a => a.AddressId.ToString() == request.AddressId);

            var announcements = await query.ToListAsync();

            var announcementList = new AnnouncementList();
            announcementList.Announcements.AddRange(announcements.Select(a => new AnnouncementUpdate
            {
                Id = a.Id.ToString(),
                AuthorId = a.AuthorId.ToString(),
                KeeperId = a.KeeperId?.ToString() ?? "",
                AnimalId = a.AnimalId.ToString(),
                KeeperProfit = a.KeeperProfit,
                IsNegotiable = a.IsNegotiable,
                Description = a.Description,
                StartTerm = a.StartTerm,
                EndTerm = a.EndTerm,
                CreatedDate = a.CreatedDate,
                FinishedDate = a.FinishedDate ?? 0,
                Status = a.Status,
                AddressId = a.AddressId.ToString()     
            }));

            return announcementList;
        }

        public override async Task<AnnouncementUpdate> UpdateAnnouncement(AnnouncementUpdate request, ServerCallContext context)
        {
            var request_type = request.GetType();

            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            Announcement? announcement;
            if (userRole == "Admin")
            {
                announcement = await _context.Announcements.FirstOrDefaultAsync(
                    a => a.Id.ToString() == request.Id);
                // To evaluate - is getting authorId from retrieved announcement, or requiring it in the request better??
                userId = announcement?.AuthorId.ToString() ?? null;
            }
            // When status is updated, the changes possible to be done are only keeperId and Status
            else if (request.HasStatus)
            {
                announcement = await _context.Announcements.FirstOrDefaultAsync(
                    a => a.Id.ToString() == request.Id);
                // Allow foreign updates if KeeperId is null or the same as requester
                if (announcement.KeeperId != null && announcement.KeeperId.ToString() != userId && announcement.AuthorId.ToString() != userId)
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "Announcement already assigned!"));
                }
                // Discard other data
                request = new AnnouncementUpdate {
                    Status = request.HasStatus ? request.Status : "",
                    KeeperId = userId
                };
                // Counts as cancellation of current keeper, can only be done by announcement author or current keeper
                if (request.Status == "created" && userId == announcement.AuthorId.ToString() || request.KeeperId == announcement.KeeperId.ToString())
                {
                    request.KeeperId = "";
                }
                // Counts as Keeper wanting to take the announcement, can be done by anyone other than the announcement author
                else if (request.Status == "pending" && request.KeeperId != announcement.AuthorId.ToString()) { }
                else if (announcement.AuthorId.ToString() == userId)    // Author changing themself is ok
                {
                    request.KeeperId = announcement.KeeperId.ToString();
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "Changing into other state than created or pending by non-owner not allowed!"));
                }
            }
            else
            {
                announcement = await _context.Announcements.FirstOrDefaultAsync(
                    a => a.Id.ToString() == request.Id && a.AuthorId.ToString() == userId);
            }

            if (announcement == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Announcement not found!"));
            }
            // Prevent author itself assigning keepers
            if (announcement.AuthorId.ToString() == userId && request.HasKeeperId && (request.KeeperId != announcement.KeeperId.ToString() && request.KeeperId != ""))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Assigning keepers by the owner is not allowed!"));
            }
            // Prevent setting keeperId when in invalid state
            if (announcement.Status == "created" && request.HasStatus && request.Status == "Created" && request.HasKeeperId && request.KeeperId != "")
            {
                // request.HasStatus is checked earlier
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Can't set keeper for a announcement with created state!"));
            }
            if (userRole != "Admin" && announcement.Status == "finished" || announcement.Status == "canceled")
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Announcement is no longer available"));
            }
            /* Even in case of Admin operation, Animal and Address should belong to the Announcement author */
            if (request.HasAnimalId)
            {
                Animal? animal = await _context.Animals.SingleOrDefaultAsync(
                    a => a.Id.ToString() == request.AnimalId && a.OwnerId == Guid.Parse(userId));
                if (animal == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound,
                        "Animal does not exist for this user"));
                }
            }
            if (request.HasAddressId)
            {
                Address? address = await _context.Addresses.SingleOrDefaultAsync(
                    a => a.Id.ToString() == request.AddressId && a.OwnerId == Guid.Parse(userId));
                if (address == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound,
                        "Address does not exist for this user."));
                }
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
                if (property.Name.Contains("Id"))
                {
                    if (property.Name == "KeeperId" && newValue as string == "")
                        newValue = null;
                    else
                        newValue = Guid.Parse(newValue as string);
                }
                    
                    
                var currentValue = typeof(Announcement).GetProperty(property.Name)?.GetValue(announcement, null);

                if (!Equals(currentValue, newValue))
                {
                    typeof(Announcement).GetProperty(property.Name)?.SetValue(announcement, newValue);
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

            var response = new AnnouncementUpdate
            {
                Id = announcement.Id.ToString()
            };

            // Include only updated fields in the response
            foreach (var propertyName in modifiedProperties)
            {
                var property = request_type.GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(Announcement).GetProperty(propertyName)?.GetValue(announcement, null);
                    if (propertyName.Contains("Id"))
                        value = value?.ToString() ?? "";
                    property.SetValue(response, value);
                }
            }

            return response;
        }

        public override async Task<AnnouncementMinimal> DeleteAnnouncement(AnnouncementMinimal request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var authorId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;


            Announcement? announcement;
            if (userRole == "Admin")
            {
                announcement = await _context.Announcements.SingleOrDefaultAsync(
                    a => a.Id.ToString() == request.Id);
            }
            else
            {
                announcement = await _context.Announcements.SingleOrDefaultAsync(
                    a => a.Id.ToString() == request.Id && a.AuthorId.ToString() == authorId);
            }
            if (announcement == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Announcement does not exist"));
            }
            try
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();

                return new AnnouncementMinimal {
                    Id = announcement.Id.ToString(),
                    AuthorId = announcement.AuthorId.ToString(),
                    KeeperId = announcement.KeeperId.ToString(),
                    AnimalId = announcement.AnimalId.ToString(),
                    AddressId = announcement.AddressId.ToString()
                };
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }           
        }
    }
}

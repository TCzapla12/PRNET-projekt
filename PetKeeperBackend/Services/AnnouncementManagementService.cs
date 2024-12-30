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

        public override async Task<AnnouncementMinimal> CreateAnnouncement(AnnouncementCreate request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

        public override async Task<AnnouncementList> GetAnnouncements(AnnouncementGet request, ServerCallContext context)
        {
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
                if (Guid.TryParse(request.Id, out var AuthorIdGuid))
                    query = query.Where(a => a.Id == AuthorIdGuid);
                else
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "GUID format invalid for AuthorId"));
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

            var announcements = query.ToList();

            var announcementList = new AnnouncementList();
            announcementList.Announcements.AddRange(announcements.Select(a => new AnnouncementCreate
            {
                Id = a.Id.ToString(),
                AuthorId = a.AuthorId.ToString(),
                KeeperId = a.KeeperId.ToString(),
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
            var authorId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Announcement? announcement = await _context.Announcements.FirstOrDefaultAsync(
                a => a.Id.ToString() == request.Id && a.AuthorId.ToString() == authorId)
                ??
                throw new RpcException(new Status(StatusCode.NotFound, "Announcement not found!"));

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
                    property.SetValue(response, value);
                }
            }

            return response;
        }

        public override async Task<AnnouncementMinimal> DeleteAnnouncement(AnnouncementMinimal request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var ownerId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                Announcement? announcement = await _context.Announcements.SingleOrDefaultAsync(a => a.Id.ToString() == request.Id)
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Announcement does not exist"));

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

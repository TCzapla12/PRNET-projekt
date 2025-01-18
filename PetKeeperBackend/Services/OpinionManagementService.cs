using Grpc.Core;
using grpc_hello_world.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Net;
using System.Security.Claims;

namespace grpc_hello_world.Services
{
    public class OpinionManagementService : OpinionService.OpinionServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserManagementService> _logger;

        public OpinionManagementService(AppDbContext context, ILogger<UserManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public override async Task<OpinionMinimal> CreateOpinion(OpinionCreate request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var userId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Admin")  // If user is Admin
            {
                if (request.HasAuthorId)
                    userId = request.AuthorId;
            }

            var announcement = await _context.Announcements.FirstOrDefaultAsync(
                a => a.Id.ToString() == request.AnnouncementId && a.AuthorId.ToString() == userId);
            if (announcement == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Announcement not found!"));
            }
            if (announcement.AuthorId.ToString() != userId)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Annoucement does not belong to the reviewer!"));
            }

            if (request.Rating > 10)  // uint32 enforces > 0 constraint
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Rating outside of allowed [0, 10] range"));
            }
            var opinion = new Opinion
            {
                AuthorId = Guid.Parse(userId),
                KeeperId = announcement.KeeperId,
                AnnouncementId = announcement.Id,
                Rating = request.Rating,
                Description = request.Description
            };

            try
            {
                _context.Opinions.Add(opinion);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }

            return new OpinionMinimal
            {
                Id = opinion.Id.ToString(),
                AnnouncementId = announcement.Id.ToString(),
                AuthorId = userId,
                KeeperId = announcement.KeeperId.ToString()
            };
        }

        [Authorize]
        public override async Task<OpinionList> GetOpinions(OpinionGet request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var authorId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.Opinions.AsQueryable();

            if (request.HasId)
                query = query.Where(o => o.Id.ToString() == request.Id);
            if (request.HasAuthorId)
                query = query.Where(o => o.AuthorId.ToString() == request.AuthorId);
            if (request.HasKeeperId)
                query = query.Where(o => o.KeeperId.ToString() == request.KeeperId);
            if (request.HasAnnouncementId)
                query = query.Where(o => o.AnnouncementId.ToString() == request.AnnouncementId);

            if (request.HasDescriptionContains && !string.IsNullOrEmpty(request.DescriptionContains))
            {
                var words = request.DescriptionContains.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    query = query.Where(a => a.Description.Contains(word));
                }

            }

            if (request.HasCreatedDateBefore)
                query = query.Where(o => o.CreatedDate <= request.CreatedDateBefore);
            if (request.HasCreatedDateAfter)
                query = query.Where(o => o.CreatedDate >= request.CreatedDateBefore);

            if (request.HasRatingLess)
                query = query.Where(o => o.Rating <= request.RatingLess);
            if (request.HasRatingMore)
                query = query.Where(o => o.Rating >= request.RatingMore);

            var opinions = await query.ToListAsync();

            var opinionList = new OpinionList();
            opinionList.Opinions.AddRange(opinions.Select(o => new OpinionUpdate
            {
                Id = o.Id.ToString(),
                AuthorId = o.AuthorId.ToString(),
                KeeperId = o.KeeperId.ToString(),
                Description = o.Description,
                CreatedDate = o.CreatedDate,
                Rating = o.Rating,
                AnnouncementId = o.AnnouncementId.ToString()
            }));

            return opinionList;
        }

        [Authorize]
        public override async Task<OpinionMinimal> DeleteOpinion(OpinionMinimal request, ServerCallContext context)
        {
            var userContext = context.GetHttpContext().User;
            var authorId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            Opinion? opinion;
            if (userRole == "Admin")
            {
                opinion = await _context.Opinions.SingleOrDefaultAsync(
                    o => o.Id.ToString() == request.Id);
            }
            else
            {
                opinion = await _context.Opinions.SingleOrDefaultAsync(
                    p => p.Id.ToString() == request.Id && p.AuthorId.ToString() == authorId);
            }
            if (opinion == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Opinion does not exist"));
            }
            try
            {
                _context.Opinions.Remove(opinion);
                await _context.SaveChangesAsync();

                return new OpinionMinimal
                {
                    Id = opinion.Id.ToString(),
                    AuthorId = opinion.AuthorId.ToString(),
                    KeeperId = opinion.KeeperId.ToString(),
                    AnnouncementId = opinion.AnnouncementId.ToString()
                };
            }
            catch (DbUpdateException e)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }
        }

        [Authorize]
        public override async Task<OpinionUpdate> UpdateOpinion(OpinionUpdate request, ServerCallContext context)
        {
            var request_type = request.GetType();

            var userContext = context.GetHttpContext().User;
            var authorId = userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = userContext.FindFirst(ClaimTypes.Role)?.Value;

            Opinion? opinion;
            if (userRole == "Admin")
            {
                opinion = await _context.Opinions.FirstOrDefaultAsync(
                    a => a.Id.ToString() == request.Id);
                authorId = opinion?.AuthorId.ToString() ?? null;
            }
            else
            {
                opinion = await _context.Opinions.FirstOrDefaultAsync(
                    a => a.Id.ToString() == request.Id && a.AuthorId.ToString() == authorId);
            }
            if (opinion == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Opinion not found!"));
            }

            var requestProperties = request_type.GetProperties();
            var modifiedProperties = new HashSet<string>();

            foreach (var property in requestProperties)
            {
                if (property.Name.Contains("Id") || property.Name == "CreatedDate")
                    continue;

                var hasProperty = request_type.GetProperty($"Has{property.Name}")?.GetValue(request, null) as bool?;
                if (hasProperty != true) // Skip if object is not present or avilability information is unavailable (null)
                    continue;

                var newValue = request_type.GetProperty(property.Name)?.GetValue(request, null);
                if (newValue == null)  // Skip if value is not available in the request (this guard might be optional, since this should not occur)
                    continue;
                
                var currentValue = typeof(Opinion).GetProperty(property.Name)?.GetValue(opinion, null);

                if (!Equals(currentValue, newValue))
                {
                    typeof(Opinion).GetProperty(property.Name)?.SetValue(opinion, newValue);
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

            var response = new OpinionUpdate
            {
                Id = opinion.Id.ToString()
            };

            // Include only updated fields in the response
            foreach (var propertyName in modifiedProperties)
            {
                var property = request_type.GetProperty(propertyName);
                if (property != null)
                {
                    var value = typeof(Opinion).GetProperty(propertyName)?.GetValue(opinion, null);
                    property.SetValue(response, value);
                }
            }

            return response;
        }
    }
}

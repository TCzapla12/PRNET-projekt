﻿using Grpc.Core;
using grpc_hello_world;
using grpc_hello_world.Services;
using grpc_hello_world.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace grpc_hello_world.Services
{
    public class AuthManagementService : AuthService.AuthServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthManagementService> _logger;
        private readonly IConfiguration _configuration;

        public AuthManagementService(AppDbContext context, ILogger<AuthManagementService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
        {
            /* override flag is needed because we get a user unauthenticated */
            User? user = await UserManagementService.GetUserAsync(request.UserId, context, _context, true);
            if (!user.IsVerified && !user.IsAdmin)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User not verified yet. Please wait for ID verification"));
            }
            if (user.IsBanned)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is banned"));
            }
            if (UserManagementService.VerifyPassword(request.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(user.Id.ToString(), user.Email, user.IsAdmin);
                return new AuthResponse { Token = token };
            }

            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));
        }

        private string GenerateJwtToken(string id, string email, bool isAdmin)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
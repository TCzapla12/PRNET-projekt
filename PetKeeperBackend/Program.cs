using grpc_hello_world;
using grpc_hello_world.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=postgres;Port=5432;Database=testdb;Username=user;Password=pass;"));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = 50 * 1024 * 1024;  // 50 MB
    options.MaxSendMessageSize = 50 * 1024 * 1024;     // 50 MB
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<UserManagementService>();
app.MapGrpcService<AuthManagementService>();
app.MapGrpcService<AddressManagementService>();
app.MapGrpcService<AnimalManagementService>();
app.MapGrpcService<AnnouncementManagementService>();
app.MapGrpcService<OpinionManagementService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
// Configure db connection

app.Run();

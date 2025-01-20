using Blazored.LocalStorage;
using grpc_hello_world;
using PetKeeperAdminWebapp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
{
    o.Address = new Uri("http://172.168.100.20:8080");
});

builder.Services.AddGrpcClient<UserService.UserServiceClient>(o =>
{
    o.Address = new Uri("http://172.168.100.20:8080");
}).ConfigureChannel(options =>
{
    options.MaxReceiveMessageSize = 50 * 1024 * 1024; // 50 MB
    options.MaxSendMessageSize = 50 * 1024 * 1024;    // 50 MB
});

builder.Services.AddGrpcClient<OpinionService.OpinionServiceClient>(o =>
{
    o.Address = new Uri("http://172.168.100.20:8080");
});

builder.Services.AddGrpcClient<AnnouncementService.AnnouncementServiceClient>(o =>
{
    o.Address = new Uri("http://172.168.100.20:8080");
});

builder.Services.AddBlazoredLocalStorage();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

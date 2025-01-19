using Microsoft.Extensions.Logging;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.View;
using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("regular.otf", "FAR");
                    fonts.AddFont("solid.otf", "FAS");
                });

            builder.Services.AddScoped<IGrpcClient, GrpcClient>();
            //builder.Services.AddScoped<IGrpcClient, MockGrpcClient>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddSingleton<DashboardViewModel>();

            builder.Services.AddTransient<OwnerPage>();
            builder.Services.AddTransient<OwnerViewModel>();
            builder.Services.AddTransient<KeeperPage>();
            builder.Services.AddTransient<KeeperViewModel>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<MorePage>();
            builder.Services.AddTransient<MoreViewModel>();

            builder.Services.AddTransient<MyPetsPage>();
            builder.Services.AddTransient<MyPetsViewModel>();
            builder.Services.AddTransient<MyAddressesPage>();
            builder.Services.AddTransient<MyAddressesViewModel>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<SearchViewModel>();

            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<RegistrationPage>();
            builder.Services.AddTransient<RegistrationViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

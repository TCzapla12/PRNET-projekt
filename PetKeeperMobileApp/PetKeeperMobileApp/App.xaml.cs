using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Application.Current!.UserAppTheme = Preferences.Get("AppTheme", "Default") switch
            {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            MainPage = new AppShell();
            CheckCredentials();
        }

        private async void CheckCredentials()
        {
            var token = await Storage.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync($"//{RouteType.Main}/{nameof(DashboardPage)}");
            }
        }
    }
}

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

            //wymuszenie jasnego motywu
            Application.Current!.UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();
            CheckCredentials();
        }

        private async void CheckCredentials()
        {
            var token = await Storage.LoadToken();

            if (!string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync($"//{RouteType.Main}/{nameof(DashboardPage)}");
            }
        }
    }
}

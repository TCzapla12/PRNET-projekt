using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(ConfirmationPage), typeof(ConfirmationPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            Routing.RegisterRoute(nameof(OwnerPage), typeof(OwnerPage));
            Routing.RegisterRoute(nameof(KeeperPage), typeof(KeeperPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(MorePage), typeof(MorePage));
            Routing.RegisterRoute(nameof(CreateAnnouncementPage), typeof(CreateAnnouncementPage));
        }
    }
}

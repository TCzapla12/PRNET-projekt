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
            Routing.RegisterRoute(nameof(CreateAnnouncementPage), typeof(CreateAnnouncementPage));
            Routing.RegisterRoute(nameof(MyAddressesPage), typeof(MyAddressesPage));
            Routing.RegisterRoute(nameof(MyPetsPage), typeof(MyPetsPage));
        }
    }
}

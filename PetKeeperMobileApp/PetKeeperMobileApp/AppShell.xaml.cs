﻿using PetKeeperMobileApp.View;

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
            Routing.RegisterRoute(nameof(EditAddressPage), typeof(EditAddressPage));
            Routing.RegisterRoute(nameof(EditAnimalPage), typeof(EditAnimalPage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
            Routing.RegisterRoute(nameof(ShowAnnouncementPage), typeof(ShowAnnouncementPage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Source == ShellNavigationSource.ShellSectionChanged)
            {
                var navigation = Shell.Current.Navigation;
                var pages = navigation.NavigationStack;
                for (var i = pages.Count - 1; i >= 1; i--)
                {
                    navigation.RemovePage(pages[i]);
                }
            }
        }
    }
}

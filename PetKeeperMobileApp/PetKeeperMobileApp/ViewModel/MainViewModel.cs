using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Templates;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel() { }

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [RelayCommand]
    async Task GoToDashboardPage(object container)
    {
        bool areAllFieldsValid = true;

        if (container is StackLayout stackLayout && stackLayout.Children[0] is Grid grid)
            foreach (var child in grid.Children)
                if (child is ValidationEntry validationEntry && !validationEntry.ValidateField())
                    areAllFieldsValid = false;

        if (!areAllFieldsValid)
            return;

        //TO DO:
        //komunikacja z backendem
        //var confirmationViewModel = new ConfirmationViewModel(StatusIcon.Error)
        //{
        //    Title = string.Empty,
        //    Description = "Wystąpił problem.",
        //    ModalCommand = new RelayCommand(async () => {
        //        await GoToDashboardPage(container);
        //    })
        //};

        //await Application.Current!.MainPage!.Navigation.PushModalAsync(new ConfirmationPage(confirmationViewModel));
        await Shell.Current.GoToAsync($"//{RouteType.Main}/{nameof(DashboardPage)}");
    }

    [RelayCommand]
    async Task GoToRegisterPage() 
    {
        await Shell.Current.GoToAsync(nameof(RegistrationPage));
    }

    [RelayCommand]
    async Task GoToForgotPasswordPage()
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }
}

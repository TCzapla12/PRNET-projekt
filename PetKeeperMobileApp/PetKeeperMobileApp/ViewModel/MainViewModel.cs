using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel() { }

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private bool isEmailErrorVisible;

    [ObservableProperty]
    private bool isPasswordErrorVisible;

    [RelayCommand]
    async Task GoToDashboardPage()
    {
        if (Validate.IsValidEmail(Email))
            IsEmailErrorVisible = false;
        else IsEmailErrorVisible = true;
        if (string.IsNullOrEmpty(Password))
            IsPasswordErrorVisible = true;
        else IsPasswordErrorVisible = false;

        if (IsEmailErrorVisible == false && IsPasswordErrorVisible == false)
        {
            //TO DO:
            //komunikacja z backendem

            var confirmationViewModel = new ConfirmationViewModel(StatusIcon.Error)
            {
                Title = string.Empty,
                Description = "Wystąpił problem.",
                ModalCommand = new RelayCommand(async () => {
                    await GoToDashboardPage();
                })
            };

            await Application.Current!.MainPage!.Navigation.PushModalAsync(new ConfirmationPage(confirmationViewModel));
        }
    }

    [RelayCommand]
    async Task GoToRegisterPage() 
    {
        //TO DO:
    }

    [RelayCommand]
    async Task GoToForgotPasswordPage()
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }
}

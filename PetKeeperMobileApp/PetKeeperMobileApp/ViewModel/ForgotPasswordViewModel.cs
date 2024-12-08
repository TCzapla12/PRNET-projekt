using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class ForgotPasswordViewModel : ObservableObject
{
    public ForgotPasswordViewModel() { }

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private bool isEmailErrorVisible;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task ResetPassword()
    {
        if (!Validate.IsValidEmail(Email))
        {
            IsEmailErrorVisible = true;
            return;
        }

        IsEmailErrorVisible = false;

        //TO DO:
        //komunikacja z backendem
        //generyczny komponent
        var confirmationViewModel = new ConfirmationViewModel(StatusIcon.Success)
        {
            Title = string.Empty,
            Description = "Na twój adres e-mail została wysłana wiadomość umożliwiająca zmianę hasła."
        };

        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ConfirmationPage(confirmationViewModel));
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Utils;

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
    }
}

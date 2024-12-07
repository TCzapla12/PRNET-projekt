using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        var confirmationViewModel = new ConfirmationViewModel()
        {
            Title = "CZE",
            Description = "",
            Status = Enums.StatusIcon.Success,
            Retry = new RelayCommand(async () => {
            //TO DO:
            })
        };

        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ConfirmationPage(confirmationViewModel));
    }
}

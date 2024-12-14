using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Templates;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class ForgotPasswordViewModel : ObservableObject
{
    public ForgotPasswordViewModel() { }

    [ObservableProperty]
    private string email;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task ResetPassword(object container)
    {
        if (container is StackLayout stackLayout && stackLayout.Children[0] is ValidationEntry validationEntry 
            && !validationEntry.ValidateField())
            return;

        //TO DO:
        //komunikacja z backendem
        //generyczny komponent
        var confirmationViewModel = new ConfirmationViewModel(StatusIcon.Success)
        {
            Title = string.Empty,
            Description = "Na twój adres e-mail została wysłana wiadomość umożliwiająca zmianę hasła.",
            ModalCommand = new RelayCommand(async () => { 
                await GoBack(); 
            })
        };

        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ConfirmationPage(confirmationViewModel));
    }
}

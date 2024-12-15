using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.View;
using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.Utils;

public class Helpers
{
    public static async Task ShowConfirmationView(StatusIcon statusIcon, string description, RelayCommand command)
    {
        var confirmationViewModel = new ConfirmationViewModel(statusIcon)
        {
            Title = string.Empty,
            Description = description,
            ModalCommand = command
        };
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ConfirmationPage(confirmationViewModel));
    }
}

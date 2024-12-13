using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.ViewModel;

public partial class ConfirmationViewModel : ObservableObject
{
    public ConfirmationViewModel(StatusIcon statusIcon) 
    {
        status = statusIcon;
        if (statusIcon == StatusIcon.Success)
            buttonText = "OK";
        else buttonText = "Spróbuj ponownie";
    }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private string buttonText;

    [ObservableProperty]
    private StatusIcon status;

    [ObservableProperty]
    private RelayCommand modalCommand;

    [RelayCommand]
    async Task ButtonAction()
    {
        await CloseModal();
        ModalCommand.Execute(null);
    }

    [RelayCommand]
    async Task CloseButtonAction()
    {
        if (Status == StatusIcon.Success)
            await ButtonAction();
        else await CloseModal();
    }

    public async Task CloseModal()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;

namespace PetKeeperMobileApp.ViewModel;

public partial class ConfirmationViewModel : ObservableObject
{
    public ConfirmationViewModel() { }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private StatusIcon status;

    [ObservableProperty]
    private RelayCommand retry;

    [RelayCommand]
    async Task CloseModal()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }
}

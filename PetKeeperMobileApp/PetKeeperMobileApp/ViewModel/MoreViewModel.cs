using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class MoreViewModel : ObservableObject
{
    public MoreViewModel() { }

    [RelayCommand]
    async Task Logout()
    {
        await Helpers.Logout();
    }

    [RelayCommand]
    async Task ShowSettings()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}

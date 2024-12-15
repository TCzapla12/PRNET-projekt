using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.ViewModel;

public partial class MoreViewModel : ObservableObject
{
    public MoreViewModel() { }

    [RelayCommand]
    async Task Logout()
    {
        Storage.RemoveToken();
        await Shell.Current.GoToAsync($"//{RouteType.Login}");
    }
}

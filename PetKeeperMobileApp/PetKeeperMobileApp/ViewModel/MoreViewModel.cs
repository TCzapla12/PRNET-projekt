using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.ViewModel;

public partial class MoreViewModel : ObservableObject
{
    public MoreViewModel() { }

    [RelayCommand]
    async Task Logout()
    {
        await Helpers.Logout();
    }
}

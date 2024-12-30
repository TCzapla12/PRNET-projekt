using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PetKeeperMobileApp.ViewModel;

public partial class ProfileViewModel : ObservableObject
{
    public ProfileViewModel() { }

    [RelayCommand]
    async Task ShowMyAddresses()
    {
        //TODO:
    }

    [RelayCommand]
    async Task ShowMyPets()
    {
        //TODO:
    }
}

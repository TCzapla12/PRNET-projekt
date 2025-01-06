using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class ProfileViewModel : ObservableObject
{
    public ProfileViewModel() { }

    [RelayCommand]
    async Task ShowMyAddresses()
    {
        await Shell.Current.GoToAsync(nameof(MyAddressesPage));
    }

    [RelayCommand]
    async Task ShowMyPets()
    {
        await Shell.Current.GoToAsync(nameof(MyPetsPage));
    }
}

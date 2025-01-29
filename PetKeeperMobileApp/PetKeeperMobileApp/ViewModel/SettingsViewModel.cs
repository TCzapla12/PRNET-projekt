using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PetKeeperMobileApp.ViewModel;

public partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel() { }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}

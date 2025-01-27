using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class KeeperViewModel : ObservableObject
{
    public KeeperViewModel() { }

    [RelayCommand]
    async Task SearchAnnouncements()
    {
        await Shell.Current.GoToAsync(nameof(SearchPage));
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class OwnerViewModel : ObservableObject
{
    public OwnerViewModel() { }

    [RelayCommand]
    async Task CreateAnnouncement()
    {
        await Shell.Current.GoToAsync(nameof(CreateAnnouncementPage));
    }
}

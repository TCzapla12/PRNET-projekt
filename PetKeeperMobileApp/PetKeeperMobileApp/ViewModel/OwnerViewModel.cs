using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class OwnerViewModel(IGrpcClient grpcClient) : ObservableObject
{
    [RelayCommand]
    async Task CreateAnnouncement()
    {
        var createAnnouncementViewModel = new CreateAnnouncementViewModel(grpcClient);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new CreateAnnouncementPage(createAnnouncementViewModel));
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.ViewModel;

public partial class OwnerViewModel(IGrpcClient grpcClient) : ObservableObject
{
    private List<AnnouncementDto> _announcementsDto;

    [ObservableProperty]
    private ObservableCollection<AnnouncementInfo> announcementList;

    [RelayCommand]
    async Task CreateAnnouncement()
    {
        var createAnnouncementViewModel = new CreateAnnouncementViewModel(grpcClient);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new CreateAnnouncementPage(createAnnouncementViewModel));
    }

    public async Task LoadDataAsync()
    {
        try
        {
            _announcementsDto = await grpcClient.GetAnnouncements();
            var announcements = new List<AnnouncementInfo>();
            foreach (AnnouncementDto announcementDto in _announcementsDto)
                announcements.Add(new AnnouncementInfo
                {
                    Id = announcementDto.Id,
                    Animal = AnimalDto.AnimalToString(await grpcClient.GetAnimal(announcementDto.AnimalId)),
                    Profit = announcementDto.Profit,
                    IsNegotiable = announcementDto.IsNegotiable,
                    Description = announcementDto.Description,
                    StartTerm = announcementDto.StartTerm,
                    EndTerm = announcementDto.EndTerm,
                    Status = announcementDto.Status,
                    Address = AddressDto.AddressToString(await grpcClient.GetAddress(announcementDto.AddressId))
                });
            AnnouncementList = new ObservableCollection<AnnouncementInfo>(announcements);
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Status.Detail, new RelayCommand(async () => {}));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () => {}));
        }
    }
}

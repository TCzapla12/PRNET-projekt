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

public partial class OwnerViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;

    public OwnerViewModel(IGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    private List<AnnouncementDto> _announcementsDto;

    [ObservableProperty]
    private ObservableCollection<AnnouncementInfo> announcementList;

    [ObservableProperty]
    private bool isErrorVisible;

    [ObservableProperty]
    private Exception exception;

    public RelayCommand RefreshData { get; }

    [RelayCommand]
    async Task CreateAnnouncement()
    {
        var createAnnouncementViewModel = new CreateAnnouncementViewModel(_grpcClient);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new CreateAnnouncementPage(createAnnouncementViewModel));
    }

    [RelayCommand]
    async Task EditAnnouncement(string id)
    {
        var editAnnouncementViewModel = new CreateAnnouncementViewModel(_grpcClient, AnnouncementList.Where(a => a.Id == id).First());
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new CreateAnnouncementPage(editAnnouncementViewModel));
    }

    [RelayCommand]
    async Task DeleteAnnouncement(string id)
    {
        try
        {
            await _grpcClient.DeleteAnnouncement(id);
            await LoadDataAsync();
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await DeleteAnnouncement(id);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await DeleteAnnouncement(id);
            }));
        }
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        var announcements = new List<AnnouncementInfo>();
        try
        {
            _announcementsDto = await _grpcClient.GetUserAnnouncements();
            foreach (AnnouncementDto announcementDto in _announcementsDto)
                announcements.Add(new AnnouncementInfo
                {
                    Id = announcementDto.Id,
                    Animal = AnimalDto.AnimalToString(await _grpcClient.GetAnimal(announcementDto.AnimalId)),
                    Profit = announcementDto.Profit,
                    IsNegotiable = announcementDto.IsNegotiable,
                    Description = announcementDto.Description,
                    StartTerm = announcementDto.StartTerm,
                    EndTerm = announcementDto.EndTerm,
                    Status = announcementDto.Status,
                    Address = AddressDto.AddressToString(await _grpcClient.GetAddress(announcementDto.AddressId))
                });
            IsErrorVisible = false;
        }
        catch (RpcException ex)
        {
            IsErrorVisible = true;
            Exception = ex;
        }
        catch (Exception ex)
        {
            IsErrorVisible = true;
            Exception = ex;
        }
        AnnouncementList = new ObservableCollection<AnnouncementInfo>(announcements);
    }
}

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

    private List<AnnouncementDto> _announcementsDto;

    public OwnerViewModel(IGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

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

    [RelayCommand]
    async Task ShowAnnouncement(string id)
    {
        var showAnnouncementViewModel = new ShowAnnouncementViewModel(_grpcClient, AnnouncementList.Where(a => a.Id == id).First(), true);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ShowAnnouncementPage(showAnnouncementViewModel));
    }

    [RelayCommand]
    async Task StartAnnouncement(string id)
    {
        try
        {
            var updateAnnouncementDto = new UpdateAnnouncementDto()
            {
                Id = id,
                Status = StatusType.Ongoing,
            };
            var message = await _grpcClient.UpdateAnnouncementStatus(updateAnnouncementDto);
            await Helpers.ShowConfirmationView(StatusIcon.Success, message, new RelayCommand(() => { }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await StartAnnouncement(id);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await StartAnnouncement(id);
            }));
        }
        await LoadDataAsync();
    }

    [RelayCommand]
    async Task RejectAnnouncement(string id)
    {
        try
        {
            var updateAnnouncementDto = new UpdateAnnouncementDto()
            {
                Id = id,
                Status = StatusType.Created,
                //KeeperId = string.Empty
            };
            //TODO: nie można nullować keeperId
            var message = await _grpcClient.UpdateAnnouncementStatus(updateAnnouncementDto);
            await Helpers.ShowConfirmationView(StatusIcon.Success, message, new RelayCommand(() => { }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await RejectAnnouncement(id);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await RejectAnnouncement(id);
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
            {
                UserDto? keeper = null;
                UserInfo? keeperInfo = null;
                if (!string.IsNullOrEmpty(announcementDto.KeeperId))
                {
                    keeper = await _grpcClient.GetUser(announcementDto.KeeperId);
                    keeperInfo = new UserInfo(keeper);
                }
                var animal = await _grpcClient.GetAnimal(announcementDto.AnimalId);
                var address = await _grpcClient.GetAddress(announcementDto.AddressId);
                announcements.Add(new AnnouncementInfo
                {
                    Id = announcementDto.Id,
                    Animal = AnimalDto.AnimalToString(animal),
                    Profit = announcementDto.Profit,
                    IsNegotiable = announcementDto.IsNegotiable,
                    Description = announcementDto.Description,
                    StartTerm = announcementDto.StartTerm,
                    EndTerm = announcementDto.EndTerm,
                    Status = announcementDto.Status,
                    Address = AddressDto.AddressToString(address),
                    Keeper = keeper?.Username ?? string.Empty,
                    KeeperInfo = keeperInfo,
                    AnimalInfo = new AnimalInfo(animal),
                    AddressInfo = new AddressInfo(address)
                });
            }
            announcements = Helpers.SortAnnouncements(announcements);
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

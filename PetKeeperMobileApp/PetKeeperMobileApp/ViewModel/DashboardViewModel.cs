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

public partial class DashboardViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;

    private UserDto _user;

    private List<AnnouncementDto> _announcementsDto;

    public DashboardViewModel(IGrpcClient grpcClient) 
    { 
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    private ImageSource photo;

    [ObservableProperty]
    private bool isErrorVisible;

    [ObservableProperty]
    private Exception exception;

    [ObservableProperty]
    private ObservableCollection<AnnouncementInfo> ownerAnnouncementList;

    [ObservableProperty]
    private ObservableCollection<AnnouncementInfo> keeperAnnouncementList;

    public RelayCommand RefreshData { get; }

    [RelayCommand]
    async Task ShowAnnouncement(string id)
    {
        AnnouncementInfo showAnnouncement;
        bool isOwnerView = false;
        if(OwnerAnnouncementList.Where(a => a.Id == id).Count() > 0)
        {
            showAnnouncement = OwnerAnnouncementList.Where(a => a.Id == id).First();
            isOwnerView = true;
        }
        else showAnnouncement = KeeperAnnouncementList.Where(a => a.Id == id).First();

        var showAnnouncementViewModel = new ShowAnnouncementViewModel(_grpcClient, showAnnouncement, isOwnerView);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ShowAnnouncementPage(showAnnouncementViewModel));
    }

    [RelayCommand]
    async Task FinishAnnouncement(string id)
    {
        await ChangeAnnouncement(id, StatusType.Finished);
    }

    [RelayCommand]
    async Task CancelAnnouncement(string id)
    {
        await ChangeAnnouncement(id, StatusType.Canceled);
    }

    private async Task ChangeAnnouncement(string id, StatusType status)
    {
        try
        {
            var updateAnnouncementDto = new UpdateAnnouncementDto()
            {
                Id = id,
                Status = status,
                //KeeperId = string.Empty
            };
            //TODO: nie można nullować keeperId
            var message = await _grpcClient.UpdateAnnouncementStatus(updateAnnouncementDto);
            await Helpers.ShowConfirmationView(StatusIcon.Success, message, new RelayCommand(async () => 
            {
                var addOpinionViewModel = new AddOpinionViewModel(_grpcClient, OwnerAnnouncementList.Where(a => a.Id == id).First());
                await Application.Current!.MainPage!.Navigation.PushModalAsync(new AddOpinionPage(addOpinionViewModel));
            }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await ChangeAnnouncement(id, status);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await ChangeAnnouncement(id, status);
            }));
        }
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        var ownerAnnouncements = new List<AnnouncementInfo>();
        var keeperAnnouncements = new List<AnnouncementInfo>();
        var myId = await Storage.GetUserId();
        try
        {
            _announcementsDto = await _grpcClient.GetAnnouncements();
            foreach (AnnouncementDto announcementDto in _announcementsDto)
            {
                if(announcementDto.Status == StatusType.Ongoing)
                {
                    if(announcementDto.OwnerId == myId)
                    {
                        var animal = await _grpcClient.GetAnimal(announcementDto.AnimalId);
                        var address = await _grpcClient.GetAddress(announcementDto.AddressId);
                        var keeper = await _grpcClient.GetUser(announcementDto.KeeperId);
                        ownerAnnouncements.Add(new AnnouncementInfo
                        {
                            Id = announcementDto.Id,
                            Animal = AnimalDto.AnimalToString(animal),
                            Profit = announcementDto.Profit,
                            IsNegotiable = announcementDto.IsNegotiable,
                            StartTerm = announcementDto.StartTerm,
                            EndTerm = announcementDto.EndTerm,
                            Status = announcementDto.Status,
                            Address = AddressDto.AddressToString(address),
                            Keeper = keeper.Username,
                            KeeperInfo = new UserInfo(keeper),
                            AnimalInfo = new AnimalInfo(animal),
                            AddressInfo = new AddressInfo(address)
                        });
                    }
                    else if(announcementDto.KeeperId == myId)
                    {
                        var address = await _grpcClient.GetAddress(announcementDto.AddressId);
                        var animal = await _grpcClient.GetAnimal(announcementDto.AnimalId);
                        var owner = await _grpcClient.GetUser(announcementDto.OwnerId);
                        keeperAnnouncements.Add(new AnnouncementInfo
                        {
                            Id = announcementDto.Id,
                            Animal = AnimalDto.AnimalToString(animal),
                            Profit = announcementDto.Profit,
                            IsNegotiable = announcementDto.IsNegotiable,
                            StartTerm = announcementDto.StartTerm,
                            EndTerm = announcementDto.EndTerm,
                            Status = announcementDto.Status,
                            Address = AddressDto.AddressToString(address),
                            Owner = owner.Username,
                            OwnerInfo = new UserInfo(owner),
                            AnimalInfo = new AnimalInfo(animal),
                            AddressInfo = new AddressInfo(address)
                        });
                    }
                }
            }
            _user = await _grpcClient.GetUser();
            var user = new UserInfo(_user);
            FirstName = user.FirstName;
            Photo = user.Photo;
            IsErrorVisible = false;
        }
        catch (RpcException ex)
        {
            IsErrorVisible = true;
            Exception = ex;
            FirstName = String.Empty;
        }
        catch (Exception ex)
        {
            IsErrorVisible = true;
            Exception = ex;
            FirstName = String.Empty;
        }
        OwnerAnnouncementList = new ObservableCollection<AnnouncementInfo>(ownerAnnouncements);
        KeeperAnnouncementList = new ObservableCollection<AnnouncementInfo>(keeperAnnouncements);
    }
}

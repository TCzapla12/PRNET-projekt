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

public partial class KeeperViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;

    private List<AnnouncementDto> _announcementsDto;

    public RelayCommand RefreshData { get; }

    public KeeperViewModel(IGrpcClient grpcClient) 
    {
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private ObservableCollection<AnnouncementInfo> announcementList;

    [ObservableProperty]
    private bool isErrorVisible;

    [ObservableProperty]
    private Exception exception;

    [RelayCommand]
    async Task SearchAnnouncements()
    {
        await Shell.Current.GoToAsync(nameof(SearchPage));
    }

    [RelayCommand]
    async Task ShowAnnouncement(string id)
    {
        var showAnnouncementViewModel = new ShowAnnouncementViewModel(_grpcClient, AnnouncementList.Where(a => a.Id == id).First());
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ShowAnnouncementPage(showAnnouncementViewModel));
    }

    [RelayCommand]
    async Task GoToFinishedAnnouncements()
    {
        await Shell.Current.GoToAsync(nameof(ShowFinalAnnouncementsPage) + "?IsFinishedView=true");
    }

    [RelayCommand]
    async Task GoToCanceledAnnouncements()
    {
        await Shell.Current.GoToAsync(nameof(ShowFinalAnnouncementsPage));
    }

    public async Task LoadDataAsync()
    {
        IsEmpty = false;
        IsLoading = true;
        var announcements = new List<AnnouncementInfo>();
        try
        {
            //TODO: nie można wyszukiwać po KeeperId
            //_announcementsDto = await _grpcClient.GetAnnouncements(keeperId: await Storage.GetUserId());
            _announcementsDto = await _grpcClient.GetAnnouncements();
            foreach (AnnouncementDto announcementDto in _announcementsDto)
            {
                //if(announcementDto.Status != Enums.StatusType.Created)
                if (announcementDto.Status != StatusType.Created && announcementDto.Status != StatusType.Finished &&
                    announcementDto.Status != StatusType.Canceled && announcementDto.KeeperId == await Storage.GetUserId())
                {
                    var address = await _grpcClient.GetAddress(announcementDto.AddressId); 
                    var owner = await _grpcClient.GetUser(announcementDto.OwnerId);
                    var animal = await _grpcClient.GetAnimal(announcementDto.AnimalId);
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
                        Owner = owner.Username,
                        OwnerInfo = new UserInfo(owner),
                        AnimalInfo = new AnimalInfo(animal),
                        AddressInfo = new AddressInfo(address)
                    });
                }    
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
        if (AnnouncementList.Count == 0 && IsErrorVisible == false)
            IsEmpty = true;
        IsLoading = false;
    }
}

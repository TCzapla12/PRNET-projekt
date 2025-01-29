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

public partial class ShowFinalAnnouncementsViewModel : ObservableObject, IQueryAttributable
{
    private IGrpcClient _grpcClient;

    private List<AnnouncementDto> _announcementsDto;

    private bool _isFinishedView;

    public ShowFinalAnnouncementsViewModel(IGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IsOwnerView", out var isOwnerView))
            IsOwnerView = bool.Parse((string)isOwnerView);

        if (query.TryGetValue("IsFinishedView", out var isFinishedView))
            _isFinishedView = bool.Parse((string)isFinishedView);

        if (_isFinishedView)
            Title = "ZAKOŃCZONE OGŁOSZENIA";
        else
            Title = "ANULOWANE OGŁOSZENIA";

        LoadDataAsync();
    }

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private bool isOwnerView;

    [ObservableProperty]
    private ObservableCollection<AnnouncementInfo> announcementList;

    [ObservableProperty]
    private bool isErrorVisible;

    [ObservableProperty]
    private Exception exception;

    public RelayCommand RefreshData { get; }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task ShowAnnouncement(string id)
    {
        var showAnnouncementViewModel = new ShowAnnouncementViewModel(_grpcClient, AnnouncementList.Where(a => a.Id == id).First(), IsOwnerView);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ShowAnnouncementPage(showAnnouncementViewModel));
    }

    public async Task LoadDataAsync()
    {
        IsEmpty = false;
        IsLoading = true;
        var announcements = new List<AnnouncementInfo>();
        try
        {
            if (IsOwnerView)
            {
                if (_isFinishedView)
                {
                    _announcementsDto = await _grpcClient.GetUserAnnouncements(StatusType.Finished);
                }
                else
                {
                    _announcementsDto = await _grpcClient.GetUserAnnouncements(StatusType.Canceled);
                }
                foreach (AnnouncementDto announcementDto in _announcementsDto)
                {
                    var keeper = await _grpcClient.GetUser(announcementDto.KeeperId);
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
                        Keeper = keeper.Username,
                        KeeperInfo = new UserInfo(keeper),
                        AnimalInfo = new AnimalInfo(animal),
                        AddressInfo = new AddressInfo(address)
                    });
                }
            }
            //TODO: nie można wyszukiwać po KeeperId
            //_announcementsDto = await _grpcClient.GetAnnouncements(keeperId: await Storage.GetUserId(), status: StatusType.Finished);
            else
            {
                if (_isFinishedView)
                {
                    _announcementsDto = await _grpcClient.GetAnnouncements(status: StatusType.Finished);
                }
                else
                {
                    _announcementsDto = await _grpcClient.GetAnnouncements(status: StatusType.Canceled);
                }
                foreach (AnnouncementDto announcementDto in _announcementsDto)
                {
                    if(announcementDto.KeeperId == await Storage.GetUserId())
                    {
                        var owner = await _grpcClient.GetUser(announcementDto.OwnerId);
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
                            Owner = owner.Username,
                            OwnerInfo = new UserInfo(owner),
                            AnimalInfo = new AnimalInfo(animal),
                            AddressInfo = new AddressInfo(address)
                        });
                    }
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

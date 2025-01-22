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

public partial class SearchViewModel : ObservableObject
{
    public SearchViewModel(IGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    private IGrpcClient _grpcClient;

    private List<AnnouncementDto> _announcementsDto;

    [ObservableProperty]
    private string searchText;

    [ObservableProperty]
    private int? minValue = null;

    [ObservableProperty]
    private int? maxValue = null;

    [ObservableProperty]
    private DateTime minimumDate = DateTime.Today;

    [ObservableProperty]
    private DateTime startTerm = DateTime.Today;

    [ObservableProperty]
    private DateTime endTerm = DateTime.Today.AddYears(1);

    [ObservableProperty]
    private ObservableCollection<AnnouncementInfo> announcementList;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task GetLocation()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await Helpers.ShowConfirmationView(StatusIcon.Error, "Brak dostępu do lokalizacji!", new RelayCommand(async () => await GetLocation()));
                return;
            }

            var location = await Geolocation.GetLocationAsync();
            if (location != null)
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location);
                SearchText = placemarks.First().Locality;
            }
            else
            {
                await Helpers.ShowConfirmationView(StatusIcon.Error, "Nie udało się uzyskać lokalizacji!", new RelayCommand(async () => await GetLocation()));
            }
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () => await GetLocation()));
        }
    }

    [RelayCommand]
    async Task SearchAnnouncements()
    {
        var announcements = new List<AnnouncementInfo>();
        try
        {
            _announcementsDto = await _grpcClient.GetAnnouncements(MinValue, MaxValue, StartTerm, EndTerm);
            foreach (AnnouncementDto announcementDto in _announcementsDto)
            {
                if(announcementDto.OwnerId != await Storage.GetUserId() && announcementDto.Status == StatusType.Created)
                {
                    var address = await _grpcClient.GetAddress(announcementDto.AddressId);
                    if(string.IsNullOrWhiteSpace(SearchText) || address.City.ToLower().Contains(SearchText.Trim().ToLower()))
                    {
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
                            Address = AddressDto.AddressMinimalToString(address),
                            Owner = owner.Username,
                            OwnerInfo = new UserInfo(owner),
                            AnimalInfo = new AnimalInfo(animal),
                            AddressInfo = new AddressInfo(address)
                        });
                    }
                }
            }
            announcements.Sort((a, b) => b.Profit.CompareTo(a.Profit));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await SearchAnnouncements();
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await SearchAnnouncements();
            }));
        }
        AnnouncementList = new ObservableCollection<AnnouncementInfo>(announcements);
    }

    [RelayCommand]
    async Task ShowAnnouncement(string id)
    {
        var showAnnouncementViewModel = new ShowAnnouncementViewModel(_grpcClient, AnnouncementList.Where(a => a.Id == id).First());
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ShowAnnouncementPage(showAnnouncementViewModel));
    }
}

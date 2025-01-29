using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Templates;
using PetKeeperMobileApp.Utils;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.ViewModel;

public partial class CreateAnnouncementViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private List<AddressDto> _addressesDto;
    private List<AnimalDto> _animalsDto;
    private AnnouncementInfo? _announcementInfo = null;

    public CreateAnnouncementViewModel(IGrpcClient grpcClient) 
    { 
        _grpcClient = grpcClient;

        Title = "Nowe ogłoszenie";
        ButtonText = "Dodaj";

        LoadDataAsync();
    }

    public CreateAnnouncementViewModel(IGrpcClient grpcClient, AnnouncementInfo announcementInfo)
    {
        _grpcClient = grpcClient;
        _announcementInfo = announcementInfo;

        Title = "Edytuj ogłoszenie";
        ButtonText = "Edytuj";

        LoadDataWithAnnouncementAsync();
    }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string buttonText;

    [ObservableProperty]
    private string profit;

    [ObservableProperty]
    private bool isNegotiable;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private ObservableCollection<string> animalList;

    [ObservableProperty]
    private ObservableCollection<string> addressList;

    [ObservableProperty]
    private string animal;

    [ObservableProperty]
    private string address;

    [ObservableProperty]
    private DateTime minimumDate = DateTime.Today;

    [ObservableProperty]
    private DateTime startTerm;

    [ObservableProperty]
    private DateTime endTerm;

    [ObservableProperty]
    private TimeSpan startTermTime;

    [ObservableProperty]
    private TimeSpan endTermTime;

    [ObservableProperty]
    private string errorText;

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task CreateEditAnnouncement(object container)
    {
        bool areAllFieldsValid = true;
        StartTerm = StartTerm.Add(StartTermTime);
        EndTerm = EndTerm.Add(EndTermTime);
        ErrorText = Validate.IsValidDate(StartTerm, EndTerm);
        if (container is StackLayout stackLayout && stackLayout.Children[0] is Grid grid)
            foreach (var child in grid.Children)
            {
                if (child is ValidationEntry validationEntry
                    && validationEntry.Type != EntryType.RepeatPassword && !validationEntry.ValidateField())
                    areAllFieldsValid = false;
                else if (child is ValidationPicker validationPicker && !validationPicker.ValidateField())
                    areAllFieldsValid = false;
                else if (child is ValidationEditor validationEditor && !validationEditor.ValidateField())
                    areAllFieldsValid = false;
            }

        if (!areAllFieldsValid || ErrorText != String.Empty)
            return;

        try
        {
            var message = String.Empty;
            if(_announcementInfo == null)
            {
                AnnouncementDto announcement = new()
                {
                    AnimalId = _animalsDto[AnimalList.IndexOf(Animal)].Id!,
                    AddressId = _addressesDto[AddressList.IndexOf(Address)].Id!,
                    Profit = uint.Parse(this.Profit),
                    IsNegotiable = this.IsNegotiable,
                    Description = this.Description,
                    StartTerm = (ulong)new DateTimeOffset(this.StartTerm).ToUnixTimeSeconds(),
                    EndTerm = (ulong)new DateTimeOffset(this.EndTerm).ToUnixTimeSeconds(),
                    Status = StatusType.Created
                };
                message = await _grpcClient.CreateAnnouncement(announcement);
            }
            else
            {
                AnnouncementDto announcement = new()
                {
                    Id = _announcementInfo.Id,
                    AnimalId = _animalsDto[AnimalList.IndexOf(Animal)].Id!,
                    AddressId = _addressesDto[AddressList.IndexOf(Address)].Id!,
                    Profit = uint.Parse(this.Profit),
                    IsNegotiable = this.IsNegotiable,
                    Description = this.Description,
                    StartTerm = (ulong)new DateTimeOffset(this.StartTerm).ToUnixTimeSeconds(),
                    EndTerm = (ulong)new DateTimeOffset(this.EndTerm).ToUnixTimeSeconds(),
                    Status = StatusType.Created
                };
                message = await _grpcClient.UpdateAnnouncement(announcement);
            }
            
            await Helpers.ShowConfirmationView(StatusIcon.Success, message, new RelayCommand(async () =>
            {
                await ButtonAction();
            }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await CreateEditAnnouncement(container);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await CreateEditAnnouncement(container);
            }));
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _addressesDto = await _grpcClient.GetAddresses();
            _animalsDto = await _grpcClient.GetAnimals();
            var addresses = new List<string>();
            var animals = new List<string>();
            foreach (var addressDto in _addressesDto)
                addresses.Add(AddressDto.AddressToString(addressDto));
            foreach (var animalDto in _animalsDto)
                animals.Add(AnimalDto.AnimalToString(animalDto));
            AddressList = new ObservableCollection<string>(addresses);
            AnimalList = new ObservableCollection<string>(animals);
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Status.Detail, new RelayCommand(async () =>
            {
                await LoadDataAsync();
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await LoadDataAsync();
            }));
        }
    }

    private async Task LoadDataWithAnnouncementAsync()
    {
        await LoadDataAsync();

        Profit = _announcementInfo!.Profit.ToString();
        IsNegotiable = _announcementInfo.IsNegotiable;
        Animal = _announcementInfo.Animal;
        Address = _announcementInfo.Address;
        Description = _announcementInfo.Description!;
        if (DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.StartTerm).DateTime >= MinimumDate)
            StartTerm = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.StartTerm).DateTime;
        else StartTerm = DateTime.Today;
        if (DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.EndTerm).DateTime >= MinimumDate)
            EndTerm = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.EndTerm).DateTime;
        else EndTerm = DateTime.Today;
        StartTermTime = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.StartTerm).TimeOfDay;
        EndTermTime = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.EndTerm).TimeOfDay;
    }
}

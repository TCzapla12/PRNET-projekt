using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;
using System.Text.RegularExpressions;

namespace PetKeeperMobileApp.ViewModel;

public partial class ShowAnnouncementViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private AnnouncementInfo _announcementInfo;

    public ShowAnnouncementViewModel(IGrpcClient grpcClient, AnnouncementInfo announcementInfo)
    {
        _grpcClient = grpcClient;
        _announcementInfo = announcementInfo;

        BindData();
    }


    [ObservableProperty]
    private string profit;

    [ObservableProperty]
    private bool isNegotiable;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private string statusIcon;

    [ObservableProperty]
    private Color statusColor;

    [ObservableProperty]
    private string statusText;

    [ObservableProperty]
    private string startTerm;

    [ObservableProperty]
    private string endTerm;

    [ObservableProperty]
    private string userText;

    [ObservableProperty]
    private string animalText;

    [ObservableProperty]
    private ImageSource ownerPhoto;

    [ObservableProperty]
    private ImageSource animalPhoto;

    [ObservableProperty]
    private string addressText;

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task ChangeAnnouncementToPending()
    {
        _announcementInfo.Status = Enums.StatusType.Pending;
        //TODO:
    }

    private void BindData()
    {
        Profit = _announcementInfo.Profit.ToString();
        IsNegotiable = _announcementInfo.IsNegotiable;
        Description = _announcementInfo.Description!;
        StartTerm = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.StartTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
        EndTerm = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.EndTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
        (StatusIcon, StatusColor, StatusText) = Helpers.GetStatusInfo(_announcementInfo.Status);
        string formattedPhone = Regex.Replace(_announcementInfo.OwnerInfo!.Phone, @"(\d{3})(?=\d)", "$1-");
        UserText = $"{_announcementInfo.OwnerInfo!.Username} ({_announcementInfo.OwnerInfo!.FirstName} {_announcementInfo.OwnerInfo!.LastName})\n" +
            $"+48 {formattedPhone}, {_announcementInfo.OwnerInfo!.Email}";
        OwnerPhoto = _announcementInfo.OwnerInfo!.Photo;
        AnimalPhoto = _announcementInfo.AnimalInfo!.Photo;
        AnimalText = _announcementInfo.Animal;
        AddressText = $"{_announcementInfo.AddressInfo!.Address1}\n{_announcementInfo.AddressInfo!.Address2}";
    }


}

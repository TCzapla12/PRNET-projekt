using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;
using System.Text.RegularExpressions;

namespace PetKeeperMobileApp.ViewModel;

public partial class ShowAnnouncementViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private AnnouncementInfo _announcementInfo;
    private bool _isOwnerView;

    public ShowAnnouncementViewModel(IGrpcClient grpcClient, AnnouncementInfo announcementInfo, bool isOwnerView = false)
    {
        _grpcClient = grpcClient;
        _announcementInfo = announcementInfo;
        _isOwnerView = isOwnerView;
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
    private ImageSource userPhoto;

    [ObservableProperty]
    private ImageSource animalPhoto;

    [ObservableProperty]
    private string addressText;

    [ObservableProperty]
    private string userLabelText;

    [ObservableProperty]
    private bool isKeeperCreatedButtonVisible;

    [ObservableProperty]
    private bool isKeeperPendingButtonVisible;

    [ObservableProperty]
    private bool isOwnerCreatedButtonVisible;

    [ObservableProperty]
    private bool isOwnerPendingButtonVisible;

    [ObservableProperty]
    private bool isOwnerOngoingButtonVisible;

    [ObservableProperty]
    private bool isNotCreatedStatus = true;

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task ChangeAnnouncementToPending()
    {
        await ChangeAnnouncement(StatusType.Pending, true);
    }

    [RelayCommand]
    async Task ChangeAnnouncementToCreated()
    {
        await ChangeAnnouncement(StatusType.Created);
    }

    [RelayCommand]
    async Task StartAnnouncement()
    {
        await ChangeAnnouncement(StatusType.Ongoing);
    }

    [RelayCommand]
    async Task RejectAnnouncement()
    {
        await ChangeAnnouncement(StatusType.Created);
    }

    [RelayCommand]
    async Task FinishAnnouncement()
    {
        await ChangeAnnouncement(StatusType.Finished);
    }

    [RelayCommand]
    async Task CancelAnnouncement()
    {
        await ChangeAnnouncement(StatusType.Canceled);
    }

    [RelayCommand]
    async Task EditAnnouncement()
    {
        var editAnnouncementViewModel = new CreateAnnouncementViewModel(_grpcClient, _announcementInfo);
        await ButtonAction();
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new CreateAnnouncementPage(editAnnouncementViewModel));
    }

    [RelayCommand]
    async Task DeleteAnnouncement()
    {
        try
        {
            var message = await _grpcClient.DeleteAnnouncement(_announcementInfo.Id!);
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Success, message, new RelayCommand(async () => { await ButtonAction(); }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await DeleteAnnouncement();
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await DeleteAnnouncement();
            }));
        }
    }

    private async Task ChangeAnnouncement(StatusType status, bool addKeeperId = false)
    {
        try
        {
            var updateAnnouncementDto = new UpdateAnnouncementDto()
            {
                Id = _announcementInfo.Id!,
                Status = status,
                //KeeperId = string.Empty
            };
            if (addKeeperId) updateAnnouncementDto.KeeperId = await Storage.GetUserId();
            //TODO: keeper nie może zmienić statusu
            //TODO: nie można nullować keeperId
            var message = await _grpcClient.UpdateAnnouncementStatus(updateAnnouncementDto);
            if(status != StatusType.Finished && status != StatusType.Canceled)
                await Helpers.ShowConfirmationView(Enums.StatusIcon.Success, message, new RelayCommand(async () => { await ButtonAction(); }));
            else
            {
                await Helpers.ShowConfirmationView(Enums.StatusIcon.Success, message, new RelayCommand(async () =>
                {
                    await ButtonAction();
                    var addOpinionViewModel = new AddOpinionViewModel(_grpcClient, _announcementInfo);
                    await Application.Current!.MainPage!.Navigation.PushModalAsync(new AddOpinionPage(addOpinionViewModel));
                }));
            }    
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await ChangeAnnouncement(status, addKeeperId);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await ChangeAnnouncement(status, addKeeperId);
            }));
        }
    }

    private void BindData()
    {
        Profit = _announcementInfo.Profit.ToString();
        IsNegotiable = _announcementInfo.IsNegotiable;
        Description = _announcementInfo.Description!;
        StartTerm = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.StartTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
        EndTerm = DateTimeOffset.FromUnixTimeSeconds((long)_announcementInfo.EndTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
        (StatusIcon, StatusColor, StatusText) = Helpers.GetStatusInfo(_announcementInfo.Status);
        AnimalPhoto = _announcementInfo.AnimalInfo!.Photo;
        AnimalText = _announcementInfo.Animal;
        AddressText = $"{_announcementInfo.AddressInfo!.Address1}\n{_announcementInfo.AddressInfo!.Address2}";
        if (_isOwnerView)
        {
            if(_announcementInfo.Status != StatusType.Created)
            {
                UserLabelText = "Opiekun: ";
                string formattedPhone = Regex.Replace(_announcementInfo.KeeperInfo!.Phone, @"(\d{3})(?=\d)", "$1-");
                UserText = $"{_announcementInfo.KeeperInfo!.Username} ({_announcementInfo.KeeperInfo!.FirstName} {_announcementInfo.KeeperInfo!.LastName})\n" +
                $"+48 {formattedPhone}, {_announcementInfo.KeeperInfo!.Email}";
                UserPhoto = _announcementInfo.KeeperInfo!.Photo;
            }
            else IsNotCreatedStatus = false;
            IsOwnerPendingButtonVisible = _announcementInfo.Status == StatusType.Pending;
            IsOwnerOngoingButtonVisible = _announcementInfo.Status == StatusType.Ongoing;
            IsOwnerCreatedButtonVisible = _announcementInfo.Status == StatusType.Created;
        }
        else
        {
            UserLabelText = "Właściciel: ";
            string formattedPhone = Regex.Replace(_announcementInfo.OwnerInfo!.Phone, @"(\d{3})(?=\d)", "$1-");
            UserText = $"{_announcementInfo.OwnerInfo!.Username} ({_announcementInfo.OwnerInfo!.FirstName} {_announcementInfo.OwnerInfo!.LastName})\n" +
            $"+48 {formattedPhone}, {_announcementInfo.OwnerInfo!.Email}";
            UserPhoto = _announcementInfo.OwnerInfo!.Photo;
            IsKeeperCreatedButtonVisible = _announcementInfo.Status == StatusType.Created;
            IsKeeperPendingButtonVisible = _announcementInfo.Status == StatusType.Pending;
        }
    }
}

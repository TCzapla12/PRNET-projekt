using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;
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
    private bool isCreatedButtonVisible;

    [ObservableProperty]
    private bool isPendingButtonVisible;

    [ObservableProperty]
    private string annId;

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task ChangeAnnouncementToPending()
    {
        try
        {
            var updateAnnouncementDto = new UpdateAnnouncementDto()
            {
                Id = _announcementInfo.Id!,
                Status = Enums.StatusType.Pending,
                KeeperId = await Storage.GetUserId()
            };
            //TODO: keeper nie może zmienić statusu
            //var message = "TO DO";
            var message = await _grpcClient.UpdateAnnouncementStatus(updateAnnouncementDto);
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Success, message, new RelayCommand(async () =>
            {
                await ButtonAction(); 
                await Shell.Current.GoToAsync("..");
            }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await ChangeAnnouncementToPending();
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await ChangeAnnouncementToPending();
            }));
        }
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
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Success, message, new RelayCommand(async () => { await ButtonAction(); }));
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
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Success, message, new RelayCommand(async () => { await ButtonAction(); }));
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
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await RejectAnnouncement(id);
            }));
        }
    }

    private void BindData()
    {
        AnnId = _announcementInfo.Id!;
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
            UserLabelText = "Opiekun: ";
            string formattedPhone = Regex.Replace(_announcementInfo.KeeperInfo!.Phone, @"(\d{3})(?=\d)", "$1-");
            UserText = $"{_announcementInfo.KeeperInfo!.Username} ({_announcementInfo.KeeperInfo!.FirstName} {_announcementInfo.KeeperInfo!.LastName})\n" +
            $"+48 {formattedPhone}, {_announcementInfo.KeeperInfo!.Email}";
            UserPhoto = _announcementInfo.KeeperInfo!.Photo;
            IsPendingButtonVisible = true;
        }
        else
        {
            UserLabelText = "Właściciel: ";
            string formattedPhone = Regex.Replace(_announcementInfo.OwnerInfo!.Phone, @"(\d{3})(?=\d)", "$1-");
            UserText = $"{_announcementInfo.OwnerInfo!.Username} ({_announcementInfo.OwnerInfo!.FirstName} {_announcementInfo.OwnerInfo!.LastName})\n" +
            $"+48 {formattedPhone}, {_announcementInfo.OwnerInfo!.Email}";
            UserPhoto = _announcementInfo.OwnerInfo!.Photo;
            IsCreatedButtonVisible = true;
        }
    }
}

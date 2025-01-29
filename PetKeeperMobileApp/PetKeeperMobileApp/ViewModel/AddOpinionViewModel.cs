using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.ViewModel;

public partial class AddOpinionViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private AnnouncementInfo _announcementInfo;

    public AddOpinionViewModel(IGrpcClient grpcClient, AnnouncementInfo announcementInfo)
    {
        _grpcClient = grpcClient;
        _announcementInfo = announcementInfo;

        Username = announcementInfo.Keeper;
    }

    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private int rating;

    [ObservableProperty]
    private Color star1Color = Color.FromArgb("#A2CCB7");

    [ObservableProperty]
    private Color star2Color = Color.FromArgb("#A2CCB7");

    [ObservableProperty]
    private Color star3Color = Color.FromArgb("#A2CCB7");

    [ObservableProperty]
    private Color star4Color = Color.FromArgb("#A2CCB7");

    [ObservableProperty]
    private Color star5Color = Color.FromArgb("#A2CCB7");

    [RelayCommand]
    async Task StarClicked(string index)
    {
        if (int.Parse(index) == Rating)
        {
            Rating--;
        }
        else Rating = int.Parse(index);
        if (Rating == 0)
        {
            Star1Color = Color.FromArgb("#A2CCB7");
            Star2Color = Color.FromArgb("#A2CCB7");
            Star3Color = Color.FromArgb("#A2CCB7");
            Star4Color = Color.FromArgb("#A2CCB7");
            Star5Color = Color.FromArgb("#A2CCB7");
        }
        else if(Rating == 1)
        {
            Star1Color = Color.FromArgb("#064849");
            Star2Color = Color.FromArgb("#A2CCB7");
            Star3Color = Color.FromArgb("#A2CCB7");
            Star4Color = Color.FromArgb("#A2CCB7");
            Star5Color = Color.FromArgb("#A2CCB7");
        }
        else if (Rating == 2)
        {
            Star1Color = Color.FromArgb("#064849");
            Star2Color = Color.FromArgb("#064849");
            Star3Color = Color.FromArgb("#A2CCB7");
            Star4Color = Color.FromArgb("#A2CCB7");
            Star5Color = Color.FromArgb("#A2CCB7");
        }
        else if (Rating == 3)
        {
            Star1Color = Color.FromArgb("#064849");
            Star2Color = Color.FromArgb("#064849");
            Star3Color = Color.FromArgb("#064849");
            Star4Color = Color.FromArgb("#A2CCB7");
            Star5Color = Color.FromArgb("#A2CCB7");
        }
        else if (Rating == 4)
        {
            Star1Color = Color.FromArgb("#064849");
            Star2Color = Color.FromArgb("#064849");
            Star3Color = Color.FromArgb("#064849");
            Star4Color = Color.FromArgb("#064849");
            Star5Color = Color.FromArgb("#A2CCB7");
        }
        else if (Rating == 5)
        {
            Star1Color = Color.FromArgb("#064849");
            Star2Color = Color.FromArgb("#064849");
            Star3Color = Color.FromArgb("#064849");
            Star4Color = Color.FromArgb("#064849");
            Star5Color = Color.FromArgb("#064849");
        }
    }

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task AddOpinion()
    {
        try
        {
            OpinionDto opinion = new()
            {
                AuthorId = await Storage.GetUserId(),
                KeeperId = _announcementInfo.KeeperInfo!.Id,
                AnnouncementId = _announcementInfo.Id!,
                Description = this.Description,
                CreatedDate = (ulong)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
                Rating = (uint)this.Rating
            };
            var message = await _grpcClient.CreateOpinion(opinion);
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Success, message, new RelayCommand(async () => { await ButtonAction(); }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await AddOpinion();
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(Enums.StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await AddOpinion();
            }));
        }
    }
}

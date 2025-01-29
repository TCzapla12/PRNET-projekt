using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.ViewModel;

public partial class ShowOpinionsViewModel : ObservableObject, IQueryAttributable
{
    private IGrpcClient _grpcClient;

    private List<OpinionDto> _opinionsDto;

    public ShowOpinionsViewModel(IGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    public RelayCommand RefreshData { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IsMyView", out var isOwnerView))
            IsMyView = bool.Parse((string)isOwnerView);

        if (IsMyView)
            Title = "OTRZYMANE OPINIE";
        else Title = "WYSTAWIONE OPINIE";
    }

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private ObservableCollection<OpinionInfo> opinionList;

    [ObservableProperty]
    private bool isMyView;

    [ObservableProperty]
    private bool isErrorVisible;

    [ObservableProperty]
    private Exception exception;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task LoadDataAsync()
    {
        IsEmpty = false;
        IsLoading = true;
        var opinions = new List<OpinionInfo>();
        try
        {
            if(IsMyView)
            {
                _opinionsDto = await _grpcClient.GetOpinions(keeperId: await Storage.GetUserId()); 
                foreach (OpinionDto opinion in _opinionsDto)
                {
                    opinions.Add(new OpinionInfo
                    {
                        Id = opinion.Id!,
                        AuthorId = opinion.AuthorId,
                        KeeperId = opinion.KeeperId,
                        Description = opinion.Description,
                        CreatedDate = opinion.CreatedDate,
                        Rating = opinion.Rating,
                        AnnouncementId = opinion.AnnouncementId,
                        AuthorInfo = new UserInfo(await _grpcClient.GetUser(opinion.AuthorId))
                    });
                }
            }
            else
            {
                _opinionsDto = await _grpcClient.GetOpinions(authorId: await Storage.GetUserId());
                foreach (OpinionDto opinion in _opinionsDto)
                {
                    opinions.Add(new OpinionInfo
                    {
                        Id = opinion.Id!,
                        AuthorId = opinion.AuthorId,
                        KeeperId = opinion.KeeperId,
                        Description = opinion.Description,
                        CreatedDate = opinion.CreatedDate,
                        Rating = opinion.Rating,
                        AnnouncementId = opinion.AnnouncementId,
                        KeeperInfo = new UserInfo(await _grpcClient.GetUser(opinion.KeeperId))
                    });
                }
            }
            opinions.Sort((a, b) => b.CreatedDate.CompareTo(a.CreatedDate));
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
        OpinionList = new ObservableCollection<OpinionInfo>(opinions);
        if (OpinionList.Count == 0 && IsErrorVisible == false)
            IsEmpty = true;
        IsLoading = false;
    }
}

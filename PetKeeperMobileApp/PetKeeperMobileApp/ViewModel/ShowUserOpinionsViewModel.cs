using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.ViewModel;

public partial class ShowUserOpinionsViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;

    private string _userId;

    private List<OpinionDto> _opinionsDto;

    public ShowUserOpinionsViewModel(IGrpcClient grpcClient, string userId, string username)
    {
        _grpcClient = grpcClient;
        _userId = userId;
        UsernameLabel = username;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
        LoadDataAsync();
    }

    public RelayCommand RefreshData { get; }

    [ObservableProperty]
    private string usernameLabel;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private ObservableCollection<OpinionInfo> opinionList;

    [ObservableProperty]
    private bool isErrorVisible;

    [ObservableProperty]
    private Exception exception;

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    public async Task LoadDataAsync()
    {
        IsEmpty = false;
        IsLoading = true;
        var opinions = new List<OpinionInfo>();
        try
        {
            _opinionsDto = await _grpcClient.GetOpinions(keeperId: _userId);
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

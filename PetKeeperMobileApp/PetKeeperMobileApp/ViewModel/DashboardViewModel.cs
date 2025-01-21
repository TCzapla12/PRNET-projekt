using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
namespace PetKeeperMobileApp.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private UserDto _user;
    public DashboardViewModel(IGrpcClient grpcClient) 
    { 
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    private bool isErrorVisible;

    [ObservableProperty]
    private Exception exception;

    public RelayCommand RefreshData { get; }

    public async Task LoadDataAsync()
    {
        try
        {
            _user = await _grpcClient.GetUser();
            FirstName = _user.FirstName;
            IsErrorVisible = false;
        }
        catch (RpcException ex)
        {
            IsErrorVisible = true;
            Exception = ex;
            FirstName = String.Empty;
        }
        catch (Exception ex)
        {
            IsErrorVisible = true;
            Exception = ex;
            FirstName = String.Empty;
        }
    }
}

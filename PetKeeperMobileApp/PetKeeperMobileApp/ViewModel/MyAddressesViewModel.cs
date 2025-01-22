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

public partial class MyAddressesViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private List<AddressDto> _addressesDto;

    public MyAddressesViewModel(IGrpcClient grpcClient) 
    {
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    [ObservableProperty]
    private ObservableCollection<AddressInfo> addresses;

    [ObservableProperty]
    private bool isCreateButtonVisible;

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
    async Task CreateAddress()
    {
        var editAddressViewModel = new EditAddressViewModel(_grpcClient);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new EditAddressPage(editAddressViewModel));
    }

    [RelayCommand]
    async Task EditAddress(string id)
    {
        var editAddressViewModel = new EditAddressViewModel(_grpcClient, _addressesDto.Where(a => a.Id == id).First());
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new EditAddressPage(editAddressViewModel));
    }

    [RelayCommand]
    async Task DeleteAddress(string id)
    {
        try
        {
            await _grpcClient.DeleteAddress(id);
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
            {
                await DeleteAddress(id);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await DeleteAddress(id);
            }));
        }
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        var addressesList = new List<AddressInfo>();
        try
        {
            _addressesDto = await _grpcClient.GetAddresses();
            _addressesDto.OrderBy(a => a.IsPrimary ?? false);
            
            for (int i = 0; i < _addressesDto.Count; i++)
            {
                addressesList.Add(new AddressInfo(i, _addressesDto[i]));
            }
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
        Addresses = new ObservableCollection<AddressInfo>(addressesList);
        IsCreateButtonVisible = Addresses.Count < 3;
    }
}

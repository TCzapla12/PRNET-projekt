using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;
using System.Collections.ObjectModel;


namespace PetKeeperMobileApp.ViewModel;

public partial class MyAddressesViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    public MyAddressesViewModel(IGrpcClient grpcClient) 
    {
        _grpcClient = grpcClient;

        LoadDataAsync();
    }

    [ObservableProperty]
    private ObservableCollection<AddressInfo> addresses;

    [ObservableProperty]
    private bool isCreateButtonVisible;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task CreateAddress()
    {
        //TO DO:
    }

    [RelayCommand]
    async Task EditAddress(int index)
    {
        //TO DO:
    }

    [RelayCommand]
    async Task DeleteAddress(int index)
    {
        //TO DO:
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var addressesDto = await _grpcClient.GetAddresses();
            addressesDto.OrderBy(a => a.IsPrimary ?? false);
            var addressesList = new List<AddressInfo>();
            for (int i = 0; i < addressesDto.Count; i++)
            {
                addressesList.Add(new AddressInfo(i, addressesDto[i]));
            }
            Addresses = new ObservableCollection<AddressInfo>(addressesList);
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
        IsCreateButtonVisible = Addresses.Count < 3;
    }
}

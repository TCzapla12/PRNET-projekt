using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Templates;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.ViewModel;

public partial class EditAddressViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;

    private AddressDto? _addressDto = null;

    public EditAddressViewModel(IGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;

        Title = "Dodaj adres";
        ButtonText = "Dodaj";
    }

    public EditAddressViewModel(IGrpcClient grpcClient, AddressDto addressDto)
    {
        _grpcClient = grpcClient; 
        _addressDto = addressDto;

        Title = "Edytuj adres";
        ButtonText = "Edytuj";

        Street = addressDto.Street;
        HouseNumber = addressDto.HouseNumber;
        if (!string.IsNullOrWhiteSpace(addressDto.ApartmentNumber))
            HouseNumber += "/" + addressDto.ApartmentNumber;
        ZipCode = addressDto.ZipCode;
        City = addressDto.City;
    }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string buttonText;

    [ObservableProperty]
    private string street;

    [ObservableProperty]
    private string houseNumber;

    [ObservableProperty]
    private string zipCode;

    [ObservableProperty]
    private string city;

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task AddEditAddress(object container)
    {
        bool areAllFieldsValid = true;

        if (container is StackLayout stackLayout && stackLayout.Children[0] is Grid grid)
            foreach (var child in grid.Children)
                if (child is ValidationEntry validationEntry)
                    if (validationEntry.Type != EntryType.RepeatPassword && !validationEntry.ValidateField())
                        areAllFieldsValid = false;

        if (!areAllFieldsValid)
            return;

        try
        {
            var message = String.Empty;
            if(_addressDto == null)
            {
                AddressDto address = new()
                {
                    Street = this.Street,
                    HouseNumber = this.HouseNumber.Split('/')[0],
                    ApartmentNumber = this.HouseNumber.Split('/').Length > 1 ? this.HouseNumber.Split('/')[1] : string.Empty,
                    City = this.City,
                    ZipCode = this.ZipCode
                };
                message = await _grpcClient.CreateAddress(address);
            }
            else
            {
                AddressDto address = new()
                {
                    Id = _addressDto.Id,
                    Street = this.Street,
                    HouseNumber = this.HouseNumber.Split('/')[0],
                    ApartmentNumber = this.HouseNumber.Split('/').Length > 1 ? this.HouseNumber.Split('/')[1] : string.Empty,
                    City = this.City,
                    ZipCode = this.ZipCode
                };
                message = await _grpcClient.UpdateAddress(address);
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
                await AddEditAddress(container);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await AddEditAddress(container);
            }));
        }
    }
}

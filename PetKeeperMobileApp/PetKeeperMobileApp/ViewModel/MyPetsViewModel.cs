using Android.Locations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Utils;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.ViewModel;

public partial class MyPetsViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    public MyPetsViewModel(IGrpcClient grpcClient) 
    {
        _grpcClient = grpcClient;

        LoadDataAsync();  
    }

    [ObservableProperty]
    private ObservableCollection<AnimalInfo> animals;

    [ObservableProperty]
    private bool isCreateButtonVisible;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task CreateAnimal()
    {
        //TODO:
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var animalsDto = await _grpcClient.GetAnimals();
            var animalsList = new List<AnimalInfo>();
            for (int i = 0; i < animalsDto.Count; i++)
            {
                animalsList.Add(new AnimalInfo(i, animalsDto[i]));
            }
            Animals = new ObservableCollection<AnimalInfo>(animalsList);
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
        IsCreateButtonVisible = Animals.Count < 10;
    }
}

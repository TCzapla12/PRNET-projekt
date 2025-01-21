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

public partial class MyPetsViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private List<AnimalDto> _animalsDto;
    public MyPetsViewModel(IGrpcClient grpcClient) 
    {
        _grpcClient = grpcClient;
        RefreshData = new RelayCommand(async () => { await LoadDataAsync(); });
    }

    [ObservableProperty]
    private ObservableCollection<AnimalInfo> animals;

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
    async Task CreateAnimal()
    {
        var createAnimalViewModel = new EditAnimalViewModel(_grpcClient);
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new EditAnimalPage(createAnimalViewModel));
    }

    [RelayCommand]
    async Task EditAnimal(string id)
    {
        var editAnimalViewModel = new EditAnimalViewModel(_grpcClient, _animalsDto.Where(a => a.Id == id).First());
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new EditAnimalPage(editAnimalViewModel));
    }

    [RelayCommand]
    async Task DeleteAnimal(string id)
    {
        try
        {
            await _grpcClient.DeleteAnimal(id);
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationViewWithHandledCodes(ex, new RelayCommand(async () =>
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
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        var animalsList = new List<AnimalInfo>();
        try
        {
            _animalsDto = await _grpcClient.GetAnimals();
            for (int i = 0; i < _animalsDto.Count; i++)
            {
                animalsList.Add(new AnimalInfo(_animalsDto[i]));
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
        Animals = new ObservableCollection<AnimalInfo>(animalsList);
        IsCreateButtonVisible = Animals.Count < 10;
    }
}

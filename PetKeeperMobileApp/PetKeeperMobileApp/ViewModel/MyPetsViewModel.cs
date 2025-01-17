﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        try
        {
            _animalsDto = await _grpcClient.GetAnimals();
            var animalsList = new List<AnimalInfo>();
            for (int i = 0; i < _animalsDto.Count; i++)
            {
                animalsList.Add(new AnimalInfo(_animalsDto[i]));
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

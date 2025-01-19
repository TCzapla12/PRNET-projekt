using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Templates;
using PetKeeperMobileApp.Utils;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.ViewModel;

public partial class EditAnimalViewModel : ObservableObject
{
    private IGrpcClient _grpcClient;
    private AnimalDto? _animalDto = null;
    private bool _isPhotoAdded;

    public EditAnimalViewModel(IGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;

        Title = "Dodaj zwierzę";
        ButtonText = "Dodaj";
        Photo = "camera.png";
    }

    public EditAnimalViewModel(IGrpcClient grpcClient, AnimalDto animalDto)
    {
        _grpcClient = grpcClient;
        _animalDto = animalDto;

        Title = "Edytuj zwierzę";
        ButtonText = "Edytuj";
        _isPhotoAdded = true;

        Name = animalDto.Name;
        AnimalType = Helpers.GetDescription(animalDto.Type);
        Photo = Helpers.AnimalBytesToImage(animalDto.Photo, animalDto.Type);
        Description = animalDto.Description ?? String.Empty;
    }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string buttonText;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string animalType;

    [ObservableProperty]
    private ImageSource photo;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private bool isPhotoErrorVisible;

    [ObservableProperty]
    private ObservableCollection<string> animalTypes =
        new(Enum.GetValues(typeof(AnimalType)).Cast<AnimalType>().Select(a => Helpers.GetDescription(a)));

    [RelayCommand]
    async Task ButtonAction()
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task AddAnimalPhoto()
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Wybierz zdjęcie zwierzęcia",
        });

        if (result != null)
        {
            var croppedImage = await Helpers.CropImage(result.FullPath, 512, 512);
            Photo = ImageSource.FromStream(() => new MemoryStream(croppedImage));
            _isPhotoAdded = true;
        }
    }

    [RelayCommand]
    async Task AddEditAnimal(object container)
    {
        IsPhotoErrorVisible = !_isPhotoAdded;
        bool areAllFieldsValid = true;

        if (container is StackLayout stackLayout && stackLayout.Children[0] is Grid grid)
            foreach (var child in grid.Children)
            {
                if (child is ValidationEntry validationEntry 
                    && validationEntry.Type != EntryType.RepeatPassword && !validationEntry.ValidateField())
                    areAllFieldsValid = false;
                else if (child is ValidationPicker validationPicker && !validationPicker.ValidateField()) 
                    areAllFieldsValid = false;
                else if (child is ValidationEditor validationEditor && !validationEditor.ValidateField())
                    areAllFieldsValid = false;
            }
                
        if (!areAllFieldsValid || IsPhotoErrorVisible)
            return;

        try
        {
            AnimalType type = Helpers.GetEnumFromDescription<AnimalType>(this.AnimalType);
            var message = String.Empty;
            if (_animalDto == null)
            {
                AnimalDto animal = new()
                {
                    Name = this.Name,
                    Type = Helpers.GetEnumFromDescription<AnimalType>(this.AnimalType),
                    Photo = await Helpers.ImageToBytes(this.Photo),
                    Description = this.Description
                };
                message = await _grpcClient.CreateAnimal(animal);
            }
            else
            {
                AnimalDto animal = new()
                {
                    Id = _animalDto.Id,
                    Name = this.Name,
                    Type = Helpers.GetEnumFromDescription<AnimalType>(this.AnimalType),
                    Photo = await Helpers.ImageToBytes(this.Photo),
                    Description = this.Description
                };
                message = await _grpcClient.UpdateAnimal(animal);
            }

            await Helpers.ShowConfirmationView(StatusIcon.Success, message, new RelayCommand(async () =>
            {
                await ButtonAction();
            }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Status.Detail, new RelayCommand(async () =>
            {
                await AddEditAnimal(container);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await AddEditAnimal(container);
            }));
        }
    }
}

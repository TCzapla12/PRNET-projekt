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

public partial class RegistrationViewModel(IGrpcClient grpcClient) : ObservableObject
{
    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string repeatPassword;

    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    private string lastName;

    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    private string street;

    [ObservableProperty]
    private string houseNumber;

    [ObservableProperty]
    private string zipCode;

    [ObservableProperty]
    private string city;

    [ObservableProperty]
    private string pesel;

    [ObservableProperty]
    private ImageSource userPhoto = "camera.png";

    [ObservableProperty]
    private ObservableCollection<ImageSource> documentPhoto = ["id_card.png", "id_card.png"];

    [ObservableProperty]
    private bool isUserPhotoErrorVisible;

    [ObservableProperty]
    private ObservableCollection<bool> isDocumentPhotoErrorVisible = [false, false];

    private bool isUserPhotoAdded;

    private List<bool> isDocumentPhotoAdded = [false, false];

    [RelayCommand]
    async Task AddUserPhoto()
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Wybierz zdjęcie użytkownika",
        });

        if (result != null)
        {
            var croppedImage = await Helpers.CropImage(result.FullPath, 512, 512);
            UserPhoto = ImageSource.FromStream(() => new MemoryStream(croppedImage));
            isUserPhotoAdded = true;
        }
    }

    [RelayCommand]
    async Task AddDocumentFrontPhoto()
    {
        await AddDocumentPhoto(0);
    }

    [RelayCommand]
    async Task AddDocumentBackPhoto()
    {
        await AddDocumentPhoto(1);
    }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task CreateAccount(object container)
    {
        IsUserPhotoErrorVisible = !isUserPhotoAdded;
        IsDocumentPhotoErrorVisible[0] = !isDocumentPhotoAdded[0];
        IsDocumentPhotoErrorVisible[1] = !isDocumentPhotoAdded[1];
        OnPropertyChanged(nameof(IsDocumentPhotoErrorVisible));

        bool areAllFieldsValid = true;

        if (container is StackLayout stackLayout && stackLayout.Children[0] is Grid grid)
            foreach (var child in grid.Children)
                if (child is ValidationEntry validationEntry)
                    if ((validationEntry.Type != EntryType.RepeatPassword && !validationEntry.ValidateField()) 
                        || (validationEntry.Type == EntryType.RepeatPassword && !validationEntry.ValidateTwoPasswords(Password)))
                            areAllFieldsValid = false;
                    
        if (!areAllFieldsValid || IsUserPhotoErrorVisible || IsDocumentPhotoErrorVisible[0] || IsDocumentPhotoErrorVisible[1])
            return;

        try
        {
            AddressDto address = new()
            {
                Street = this.Street,
                HouseNumber = this.HouseNumber.Split('/')[0],
                ApartmentNumber = this.HouseNumber.Split('/').Length > 1 ? this.HouseNumber.Split('/')[1] : string.Empty,
                City = this.City,
                ZipCode = this.ZipCode
            };
            RegisterDto user = new()
            {
                Email = this.Email,
                Username = this.Username,
                Password = this.Password,
                FirstName = this.FirstName,
                LastName = this.LastName,
                PrimaryAddress = address,
                Phone = this.PhoneNumber,
                Pesel = this.Pesel,
                AvatarPng = await Helpers.ImageToBytes(this.UserPhoto),
                DocumentPngs = [await Helpers.ImageToBytes(this.documentPhoto[0]), await Helpers.ImageToBytes(this.documentPhoto[1])]
            };
            var message = await grpcClient.Register(user);

            await Helpers.ShowConfirmationView(StatusIcon.Success, message, new RelayCommand(async () =>
            {
                await GoBack();
            }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Status.Detail, new RelayCommand(async () =>
            {
                await CreateAccount(container);
            }));
        }
        catch (Exception ex) 
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await CreateAccount(container);
            }));
        }
    }

    private async Task AddDocumentPhoto(int index)
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Wybierz dokument tożsamości",
        });

        if (result != null)
        {
            DocumentPhoto[index] = ImageSource.FromFile(result.FullPath);
            isDocumentPhotoAdded[index] = true;
            OnPropertyChanged(nameof(DocumentPhoto));
        }
    }
}

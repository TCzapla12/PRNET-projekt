using Android.Graphics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.ViewModel;

public partial class RegistrationViewModel : ObservableObject
{
    public RegistrationViewModel() { }

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
    private ImageSource userPhoto = "user_plus.png";

    [ObservableProperty]
    private ObservableCollection<ImageSource> documentPhoto = ["id_card.png", "id_card.png"];


    //TO DO:
    //obsługa inputów (generyczne?), zdjęcia, nextPage
    //problem z bindowaniem dwóch obrazków, tekst

    [RelayCommand]
    async Task AddUserPhoto()
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Wybierz zdjęcie użytkownika",
        });

        if (result != null)
        {
            var croppedImage = await CropImage(result.FullPath, 512, 512);
            UserPhoto = ImageSource.FromStream(() => new MemoryStream(croppedImage));
        }
    }

    [RelayCommand]
    async Task AddDocumentPhoto(string index)
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Wybierz dokument tożsamości",
        });

        if (result != null)
        {
            DocumentPhoto[int.Parse(index)] = ImageSource.FromFile(result.FullPath);
            OnPropertyChanged(nameof(DocumentPhoto));
        }
    }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task CreateAccount()
    {
        // TO DO:
    }

    private async Task<byte[]> CropImage(string imagePath, int width, int height)
    {
        using var originalImage = SKBitmap.Decode(imagePath);

        int cropSize = Math.Min(originalImage.Width, originalImage.Height);
        int x = (originalImage.Width - cropSize) / 2;
        int y = (originalImage.Height - cropSize) / 2;

        using var croppedBitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(croppedBitmap);

        canvas.DrawBitmap(originalImage,
            new SKRect(x, y, x + cropSize, y + cropSize),
            new SKRect(0, 0, width, height));

        using var image = SKImage.FromBitmap(croppedBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }
}

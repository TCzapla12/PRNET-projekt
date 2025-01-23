using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.View;
using PetKeeperMobileApp.ViewModel;
using SkiaSharp;
using System.ComponentModel;

namespace PetKeeperMobileApp.Utils;

public class Helpers
{
    public static async Task ShowConfirmationView(StatusIcon statusIcon, string description, RelayCommand command, string? text = null)
    {
        var confirmationViewModel = new ConfirmationViewModel(statusIcon, text)
        {
            Title = string.Empty,
            Description = description,
            ModalCommand = command
        };
        await Application.Current!.MainPage!.Navigation.PushModalAsync(new ConfirmationPage(confirmationViewModel));
    }

    public static async Task ShowConfirmationViewWithHandledCodes(RpcException exception, RelayCommand command)
    {
        if (exception.StatusCode == StatusCode.Unauthenticated)
            await Helpers.ShowConfirmationView(StatusIcon.Error, Wordings.LOGOUT_DESCRIPTION, new RelayCommand(async () => { await Helpers.Logout(); }), Wordings.LOGOUT);
        else
            await Helpers.ShowConfirmationView(StatusIcon.Error, exception.Status.Detail, command);
    }

    public static ImageSource BytesToImage(byte[] bytes, string defaultImage = "question.png")
    {
        if (bytes == null || bytes.Length == 0)
            return defaultImage;

        return ImageSource.FromStream(() => new MemoryStream(bytes));
    }

    public static async Task<byte[]> ImageToBytes(ImageSource image)
    {
        if (image is FileImageSource fileImageSource)
        {
            var filePath = fileImageSource.File;
            return await File.ReadAllBytesAsync(filePath);
        }
        else
        {
            var stream = await ((StreamImageSource)image).Stream(CancellationToken.None);
            byte[] bytesAvailable = new byte[stream.Length];
            stream.Read(bytesAvailable, 0, bytesAvailable.Length);
            return bytesAvailable;
        }
    }

    public static ImageSource AnimalBytesToImage(byte[] bytes, AnimalType type) => type switch
    {
        AnimalType.Cat => Helpers.BytesToImage(bytes, "cat.png"),
        AnimalType.Dog => Helpers.BytesToImage(bytes, "dog.png"),
        AnimalType.Other => Helpers.BytesToImage(bytes),
        _ => Helpers.BytesToImage(bytes),
    };

    public static async Task<byte[]> CropImage(string imagePath, int width, int height)
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


    public static string GetDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                             .FirstOrDefault() as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }

    public static TEnum GetEnumFromDescription<TEnum>(string description) where TEnum : Enum
    {
        var enumType = typeof(TEnum);

        foreach (var field in enumType.GetFields())
        {
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            if (attribute != null && attribute.Description == description)
            {
                return (TEnum)field.GetValue(null);
            }
        }

        throw new ArgumentException();
    }

    public static (string, Color, string) GetStatusInfo(StatusType status) => 
        status switch
        {
            StatusType.Created => ("\uf111", Color.FromArgb("#064849"), "aktualne"),
            StatusType.Pending => ("\uf28b", Color.FromArgb("#EF8159"), "oczekiwanie"),
            StatusType.Accepted => ("\uf058", Color.FromArgb("#064849"), "potwierdzone"),
            StatusType.Ongoing => ("\uf144", Color.FromArgb("#A2CCB7"), "trwa"),
            StatusType.Finished => ("\uf28d", Colors.Green, "zakończone"),
            StatusType.Canceled => ("\uf057", Colors.Red, "anulowane"),
            _ => ("\uf059", Colors.Red, "nieznany status"),
        };

    public static List<AnnouncementInfo> SortAnnouncements(List<AnnouncementInfo> announcements)
    {
        var customOrder = new[] { StatusType.Ongoing, StatusType.Pending, StatusType.Created, StatusType.Finished, StatusType.Canceled };
        return announcements.OrderBy(a => Array.IndexOf(customOrder, a.Status)).ToList();
    }

    public static async Task Logout()
    {
        Storage.RemoveCredentials();
        await Shell.Current.GoToAsync($"//{RouteType.Login}");
    }
}

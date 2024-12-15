using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Templates;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class MainViewModel(IGrpcClient grpcClient) : ObservableObject
{
    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [RelayCommand]
    async Task GoToDashboardPage(object container)
    {
        bool areAllFieldsValid = true;

        if (container is StackLayout stackLayout && stackLayout.Children[0] is Grid grid)
            foreach (var child in grid.Children)
                if (child is ValidationEntry validationEntry && !validationEntry.ValidateField())
                    areAllFieldsValid = false;

        if (!areAllFieldsValid)
            return;

        try
        {
            AuthDto dto = new()
            {
                Email = this.Email,
                HashPassword = Security.HashMD5(this.Password)
            };

            var token = await grpcClient.Login(dto);

            await Storage.SaveToken(token);

            await Shell.Current.GoToAsync($"//{RouteType.Main}/{nameof(DashboardPage)}");
        }
        catch (RpcException ex) {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Status.Detail, new RelayCommand(async () =>
            {
                await GoToDashboardPage(container);
            }));
        }
        catch (Exception ex) {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await GoToDashboardPage(container);
            }));
        }
    }

    [RelayCommand]
    async Task GoToRegisterPage() 
    {
        await Shell.Current.GoToAsync(nameof(RegistrationPage));
    }

    [RelayCommand]
    async Task GoToForgotPasswordPage()
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }
}

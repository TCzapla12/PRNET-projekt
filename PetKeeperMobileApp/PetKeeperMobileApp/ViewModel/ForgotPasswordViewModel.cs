using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Services;
using PetKeeperMobileApp.Templates;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;

namespace PetKeeperMobileApp.ViewModel;

public partial class ForgotPasswordViewModel(IGrpcClient grpcClient) : ObservableObject
{
    [ObservableProperty]
    private string email;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task ResetPassword(object container)
    {
        if (container is StackLayout stackLayout && stackLayout.Children[0] is ValidationEntry validationEntry 
            && !validationEntry.ValidateField())
            return;

        try
        {
            var message = await grpcClient.ResetPassword(Email);

            await Helpers.ShowConfirmationView(StatusIcon.Success, message, new RelayCommand(async () =>
            {
                await GoBack();
            }));
        }
        catch (RpcException ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Status.Detail, new RelayCommand(async () =>
            {
                await ResetPassword(container);
            }));
        }
        catch (Exception ex)
        {
            await Helpers.ShowConfirmationView(StatusIcon.Error, ex.Message, new RelayCommand(async () =>
            {
                await ResetPassword(container);
            }));
        }
    }
}

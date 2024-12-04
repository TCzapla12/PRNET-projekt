using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PetKeeperMobileApp.ViewModel;

public partial class ForgotPasswordViewModel : ObservableObject
{
    public ForgotPasswordViewModel() { }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}

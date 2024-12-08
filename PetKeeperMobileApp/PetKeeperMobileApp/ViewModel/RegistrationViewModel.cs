using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    private string avatarUrl;

    [ObservableProperty]
    private string documentUrl;

    //TO DO:
    //obsługa inputów (generyczne?), zdjęcia, nextPage

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
}

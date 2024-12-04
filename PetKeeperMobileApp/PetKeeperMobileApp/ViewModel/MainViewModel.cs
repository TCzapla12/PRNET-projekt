using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.View;
using System.Text.RegularExpressions;

namespace PetKeeperMobileApp.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel() { }

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string loginErrorMessage;

    [ObservableProperty]
    private bool isEmailErrorVisible;

    [ObservableProperty]
    private bool isPasswordErrorVisible;

    [ObservableProperty]
    private bool isloginErrorVisible;

    [RelayCommand]
    void OnClickLoginBtn()
    {
        if (IsValidEmail(Email))
            IsEmailErrorVisible = false;
        else IsEmailErrorVisible = true;
        if (string.IsNullOrEmpty(Password))
            IsPasswordErrorVisible = true;
        else IsPasswordErrorVisible = false;

        if (IsEmailErrorVisible == false && IsPasswordErrorVisible == false)
        {
            //TO DO:
            //komunikacja z backendem
            LoginErrorMessage = "TO DO";
            IsloginErrorVisible = true;

        }
    }

    [RelayCommand]
    void OnClickRegisterBtn() 
    {
        //TO DO:
        IsloginErrorVisible = false;
        LoginErrorMessage = string.Empty;
    }

    [RelayCommand]
    async Task GoToForgotPasswordPage()
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email);
    }
}

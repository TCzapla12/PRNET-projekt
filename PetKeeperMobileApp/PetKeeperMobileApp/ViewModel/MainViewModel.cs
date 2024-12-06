﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.View;

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
    void GoToDashboardPage()
    {
        if (Validate.IsValidEmail(Email))
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
    void GoToRegisterPage() 
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
}
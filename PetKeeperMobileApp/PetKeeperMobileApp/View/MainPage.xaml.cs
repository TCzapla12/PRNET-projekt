﻿using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class MyPetsPage : ContentPage
{
	public MyPetsPage(MyPetsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MyPetsViewModel vm)
            _ = vm.LoadDataAsync();
    }
}
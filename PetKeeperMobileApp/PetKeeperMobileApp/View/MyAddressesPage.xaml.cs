using PetKeeperMobileApp.Utils;
using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class MyAddressesPage : ContentPage
{
	public MyAddressesPage(MyAddressesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MyAddressesViewModel vm)
            _ = vm.LoadDataAsync();
    }
}
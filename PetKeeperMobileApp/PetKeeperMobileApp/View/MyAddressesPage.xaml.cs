using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class MyAddressesPage : ContentPage
{
	public MyAddressesPage(MyAddressesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
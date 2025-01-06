using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class MyPetsPage : ContentPage
{
	public MyPetsPage(MyPetsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
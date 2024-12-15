using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class OwnerPage : ContentPage
{
	public OwnerPage(OwnerViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
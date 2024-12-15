using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class ConfirmationPage : ContentPage
{
	public ConfirmationPage(ConfirmationViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
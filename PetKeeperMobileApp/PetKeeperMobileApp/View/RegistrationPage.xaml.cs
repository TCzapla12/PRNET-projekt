using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage(RegistrationViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
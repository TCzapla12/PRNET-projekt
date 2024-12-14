using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage(ForgotPasswordViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
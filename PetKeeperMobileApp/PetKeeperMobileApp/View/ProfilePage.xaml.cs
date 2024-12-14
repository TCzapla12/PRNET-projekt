using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
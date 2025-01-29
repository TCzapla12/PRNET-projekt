using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
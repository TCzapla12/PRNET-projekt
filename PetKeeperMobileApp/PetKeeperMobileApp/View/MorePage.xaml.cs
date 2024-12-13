using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class MorePage : ContentPage
{
	public MorePage(MoreViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
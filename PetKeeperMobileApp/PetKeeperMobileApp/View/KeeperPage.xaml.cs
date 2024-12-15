using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class KeeperPage : ContentPage
{
	public KeeperPage(KeeperViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class EditAddressPage : ContentPage
{
	public EditAddressPage(EditAddressViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}
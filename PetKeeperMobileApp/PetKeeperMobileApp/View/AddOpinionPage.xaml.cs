using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class AddOpinionPage : ContentPage
{
	public AddOpinionPage(AddOpinionViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}
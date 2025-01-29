using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class ShowUserOpinionsPage : ContentPage
{
	public ShowUserOpinionsPage(ShowUserOpinionsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
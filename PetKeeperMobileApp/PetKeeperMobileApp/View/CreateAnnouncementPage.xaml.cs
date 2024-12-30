using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class CreateAnnouncementPage : ContentPage
{
	public CreateAnnouncementPage(CreateAnnouncementViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
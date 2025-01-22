using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class ShowAnnouncementPage : ContentPage
{
	public ShowAnnouncementPage(ShowAnnouncementViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
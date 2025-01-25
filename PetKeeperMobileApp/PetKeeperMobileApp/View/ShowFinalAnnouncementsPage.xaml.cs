using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class ShowFinalAnnouncementsPage : ContentPage
{
	public ShowFinalAnnouncementsPage(ShowFinalAnnouncementsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}
using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel vm)
            _ = vm.LoadDataAsync();            
    }
}
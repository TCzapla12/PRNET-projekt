using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class OwnerPage : ContentPage
{
	public OwnerPage(OwnerViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is OwnerViewModel vm)
            _ = vm.LoadDataAsync();
    }
}
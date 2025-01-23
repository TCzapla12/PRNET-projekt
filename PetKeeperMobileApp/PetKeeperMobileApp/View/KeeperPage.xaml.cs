using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class KeeperPage : ContentPage
{
	public KeeperPage(KeeperViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is KeeperViewModel vm)
            _ = vm.LoadDataAsync();
    }
}
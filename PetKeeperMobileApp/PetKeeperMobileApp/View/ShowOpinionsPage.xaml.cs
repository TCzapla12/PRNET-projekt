using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class ShowOpinionsPage : ContentPage
{
	public ShowOpinionsPage(ShowOpinionsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ShowOpinionsViewModel vm)
            _ = vm.LoadDataAsync();
    }
}
using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class SearchPage : ContentPage
{
	public SearchPage(SearchViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
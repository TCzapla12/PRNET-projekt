using PetKeeperMobileApp.ViewModel;

namespace PetKeeperMobileApp.View;

public partial class EditAnimalPage : ContentPage
{
	public EditAnimalPage(EditAnimalViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}
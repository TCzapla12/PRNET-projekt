using Android.OS;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PetKeeperMobileApp.ViewModel;

public partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel() 
    {
        SelectedTheme = Preferences.Get("AppTheme", "Default");
    }

    private string _selectedTheme;

    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme != value && value != null)
            {
                _selectedTheme = value;
                OnPropertyChanged();
                ThemeChanged(value);
            }
        }
    }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void ThemeChanged(string theme)
    {
        Application.Current!.UserAppTheme = theme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified,
        };
        
        Preferences.Set("AppTheme", theme);
    }
}

using PetKeeperMobileApp.Utils;
using System.Collections.ObjectModel;

namespace PetKeeperMobileApp.Templates;

public partial class ValidationPicker : ContentView
{
	public ValidationPicker()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(ValidationEntry), string.Empty);

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<string>), typeof(ValidationEntry), new ObservableCollection<string>());

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(string), typeof(ValidationEntry), string.Empty, BindingMode.TwoWay);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public ObservableCollection<string> ItemsSource
    {
        get => (ObservableCollection<string>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string SelectedItem
    {
        get => (string)GetValue(SelectedItemProperty);
        set  
        {
            if (!string.IsNullOrEmpty(value))
                SetValue(SelectedItemProperty, value);
        }
    }

    private string errorText;

    public string ErrorText
    {
        get => errorText;
        set
        {
            if (errorText != value)
            {
                errorText = value;
                OnPropertyChanged();
            }
        }
    }

    public bool ValidateField()
    {
        if (SelectedItem != null)
            ErrorText = string.Empty;
        else
            ErrorText = Wordings.OBLIGATORY;

        return string.IsNullOrEmpty(ErrorText);
    }
}
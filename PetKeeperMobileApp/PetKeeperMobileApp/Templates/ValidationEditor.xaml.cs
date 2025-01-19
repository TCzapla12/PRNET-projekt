using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Templates;

public partial class ValidationEditor : ContentView
{
	public ValidationEditor()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ValidationEntry), string.Empty);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ValidationEntry), string.Empty, BindingMode.TwoWay);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
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
        ErrorText = Validate.IsObligatoryText(Text);

        return string.IsNullOrEmpty(ErrorText);
    }
}
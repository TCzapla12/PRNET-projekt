using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Templates;

public partial class ValidationEntry : ContentView
{
    public ValidationEntry() 
    { 
        InitializeComponent();
    }

    public static readonly BindableProperty TypeProperty =
        BindableProperty.Create(nameof(Type), typeof(EntryType), typeof(ValidationEntry), EntryType.Text);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ValidationEntry), string.Empty);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ValidationEntry), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(ValidationEntry), Keyboard.Default);

    public static readonly BindableProperty IsSpellCheckProperty =
        BindableProperty.Create(nameof(IsSpellCheck), typeof(bool), typeof(ValidationEntry), true);

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(ValidationEntry), false);

    public EntryType Type
    {
        get => (EntryType)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

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
    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public bool IsSpellCheck
    {
        get => (bool)GetValue(IsSpellCheckProperty);
        set => SetValue(IsSpellCheckProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
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
        ErrorText = Type switch
        {
            EntryType.Text => Validate.IsObligatoryText(Text),
            EntryType.Email => Validate.IsValidEmail(Text),
            EntryType.Password => Validate.IsValidPassword(Text),
            EntryType.RepeatPassword => string.Empty,
            EntryType.Telephone => Validate.IsValidPhoneNumber(Text),
            EntryType.BuildingApartment => Validate.IsValidBuildingApartmentNumber(Text),
            EntryType.ZipCode => Validate.IsValidZipCode(Text),
            _ => string.Empty,
        };

        return string.IsNullOrEmpty(ErrorText);
    }

    public bool ValidateTwoPasswords(string password)
    {
        ErrorText = Validate.IsSamePassword(Text, password);

        return string.IsNullOrEmpty(ErrorText);
    }
}
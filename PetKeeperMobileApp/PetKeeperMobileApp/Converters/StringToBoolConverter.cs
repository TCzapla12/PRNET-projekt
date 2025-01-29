using System.Globalization;

namespace PetKeeperMobileApp.Converters;

public class StringToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null && value.ToString() == parameter!.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value! ? parameter!.ToString() : null;
    }
}

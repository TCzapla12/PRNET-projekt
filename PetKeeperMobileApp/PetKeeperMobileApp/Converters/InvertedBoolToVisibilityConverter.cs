using System.Globalization;

namespace PetKeeperMobileApp.Converters;

public class InvertedBoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is bool valueToInvert)
        {
            return !valueToInvert;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

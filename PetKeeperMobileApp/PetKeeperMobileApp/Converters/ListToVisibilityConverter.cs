using System.Collections;
using System.Globalization;

namespace PetKeeperMobileApp.Converters;

public class ListToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        
        if (value is ICollection collection && collection.Count>0)
        {
            return true;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

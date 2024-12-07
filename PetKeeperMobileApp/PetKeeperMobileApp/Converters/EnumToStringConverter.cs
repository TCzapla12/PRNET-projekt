using PetKeeperMobileApp.Enums;
using System.Globalization;

namespace PetKeeperMobileApp.Converters;

class EnumToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is StatusIcon status)
        {
            return status switch
            {
                StatusIcon.Success => "success",
                StatusIcon.Error => "error",
                _ => "error",
            };
        }

        return "error";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

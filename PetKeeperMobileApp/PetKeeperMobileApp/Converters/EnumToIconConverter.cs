using PetKeeperMobileApp.Enums;
using System.Globalization;

namespace PetKeeperMobileApp.Converters;

class EnumToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is StatusIcon status)
        {
            return status switch
            {
                StatusIcon.Success => "circle_check.png",
                StatusIcon.Error => "circle_exclamation.png",
                _ => "circle_exclamation.png",
            };
        }

        return "circle_exclamation.png";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using PetKeeperMobileApp.Enums;
using System.Globalization;

namespace PetKeeperMobileApp.Converters;

class EnumToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        StatusIcon status => status switch
        {
            StatusIcon.Success => "circle_check.png",
            StatusIcon.Error => "circle_exclamation.png",
            _ => "circle_exclamation.png"
        },
        AnimalType animal => animal switch
        {
            AnimalType.Cat => "cat.png",
            AnimalType.Dog => "dog.png",
            AnimalType.Other => "question.png",
            _ => "question.png",
        },
        _ => "circle_exclamation.png"
    };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

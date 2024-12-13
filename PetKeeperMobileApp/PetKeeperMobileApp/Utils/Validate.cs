using System.Text.RegularExpressions;

namespace PetKeeperMobileApp.Utils;

class Validate
{
    private const string OBLIGATORY = "Pole obowiązkowe!";
    private const string BAD_EMAIL = "Niepoprawny adres e-mail!";
    private const string BAD_PASSWORD = "Hasło powinno składać się od 8 do 20 znaków!";
    private const string DIFFERENT_PASSWORDS = "Hasła nie są indentyczne!";
    private const string BAD_PHONE = "Niepoprawny numer telefonu!";
    private const string BAD_BUILDING = "Niepoprawny nr domu/m!";
    private const string BAD_ZIPCODE = "Niepoprawny kod pocztowy!";

    public static string IsValidEmail(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return OBLIGATORY;

        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!regex.IsMatch(field)) return BAD_EMAIL;
        return string.Empty;
    }

    public static string IsValidPassword(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return OBLIGATORY;

        var regex = new Regex(@"^.{8,20}$");
        if (!regex.IsMatch(field)) return BAD_PASSWORD;
        return string.Empty;
    }

    public static string IsSamePassword(string field1, string field2)
    {
        if (string.IsNullOrWhiteSpace(field1))
            return OBLIGATORY;
        if (field1 != field2) return DIFFERENT_PASSWORDS;
        return string.Empty;
    }

    public static string IsValidPhoneNumber(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return OBLIGATORY;
        var regex = new Regex(@"^\d{9}$");
        if (!regex.IsMatch(field)) return BAD_PHONE;
        return string.Empty;
    }

    public static string IsValidBuildingApartmentNumber(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return OBLIGATORY;
        var regex = new Regex(@"^[0-9]+[A-Za-z]?(/[0-9]+[A-Za-z]?)?$");
        if (!regex.IsMatch(field)) return BAD_BUILDING;
        return string.Empty;
    }

    public static string IsValidZipCode(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return OBLIGATORY;
        var regex = new Regex(@"^\d{2}-\d{3}$");
        if (!regex.IsMatch(field)) return BAD_ZIPCODE;
        return string.Empty;
    }

    public static string IsObligatoryText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return OBLIGATORY;
        return string.Empty;
    }
}

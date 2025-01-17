using System.ComponentModel;

namespace PetKeeperMobileApp.Enums;

public enum AnimalType
{
    [Description("Kot 🐱")]
    Cat,
    [Description("Pies 🐶")]
    Dog,
    [Description("Inne 🐾")]
    Other
}

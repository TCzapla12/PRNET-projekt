using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Models;

public class UserInfo
{
    public UserInfo(UserDto userDto)
    {
        Id = userDto.Id;
        Email = userDto.Email;
        Username = userDto.Username;
        FirstName = userDto.FirstName;
        LastName = userDto.LastName;
        Phone = userDto.Phone;
        Photo = Helpers.BytesToImage(userDto.Photo);

    }
    public string Id { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public ImageSource Photo { get; set; }
}

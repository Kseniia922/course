
namespace RestaurantApp.Models;

public class AppUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = "";

    public string Role { get; set; } = "";

    public bool IsActive { get; set; }
}

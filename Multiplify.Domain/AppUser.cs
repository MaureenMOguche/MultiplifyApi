using Microsoft.AspNetCore.Identity;

namespace Multiplify.Domain;
public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

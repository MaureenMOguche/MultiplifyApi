using Microsoft.AspNetCore.Identity;

namespace Multiplify.Domain.User;
public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsRoleSet { get; set; } = false;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshExpiry { get; set; }
    public string? ProfilePicture { get; set; }

    public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
    public string? Role { get; set; }

    public string? FundProviderInterests { get; set; }
    public string? MarketExplorerInterests { get; set; }
    public string? Biography { get; set; }

    public ICollection<Review>? Reviews { get; set; }
    public int? AverageRating => Reviews?.Count > 0 ? Reviews.Sum(x => x.Rate) / Reviews.Count : 0;
    public string FullName => $"{FirstName} {LastName}";
}

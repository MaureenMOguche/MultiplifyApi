using Multiplify.Domain.Common;
using Multiplify.Domain.User;

namespace Multiplify.Domain;
public class Fund : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MininumRange { get; set; }
    public decimal MaximumRange { get; set; }
    public string Categories { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
    public DateTime ApplicationDeadline { get; set; }

    public string FunderId { get; set; } = string.Empty;
    public AppUser Funder { get; set; } = null!;

    public ICollection<FundApplication> Applicants { get; set; } = [];
}

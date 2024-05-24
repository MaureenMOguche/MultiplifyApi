using Multiplify.Domain.Common;
using Multiplify.Domain.Enums;
using Multiplify.Domain.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Multiplify.Domain;
public class BusinessInformation : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BusinessStage Stage { get; set; }
    public string Industry { get; set; } = string.Empty;
    public string Categories { get; set; } = string.Empty;
    public decimal AverageIncome { get; set; }
    public string? BusinessLogo { get; set; }
    public string? Certifications { get; set; }

    public string EntreprenuerId { get; set; } = string.Empty;
    [ForeignKey(nameof(EntreprenuerId))]
    public virtual AppUser Entreprenuer { get; set; } = new();
}

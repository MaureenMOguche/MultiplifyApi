using Multiplify.Domain.Common;
using Multiplify.Domain.Enums;
using Multiplify.Domain.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Multiplify.Domain;
public class FundApplication : AuditableEntity
{
    public int Id { get; set; }

    [ForeignKey(nameof(Applicant))]
    public string ApplicantId { get; set; } = string.Empty;
    public AppUser Applicant { get; set; } = null!;

    [ForeignKey(nameof(Fund))]
    public int FundId { get; set; }
    public Fund Fund { get; set; } = null!;

    public FundApplicationStatus Status { get; set; } = FundApplicationStatus.Pending;
}


namespace Multiplify.Domain.Common;
public class AuditableEntity
{
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime LastModifiedOn { get; set; }
    public string LastModifiedBy { get; set; } = string.Empty;
}

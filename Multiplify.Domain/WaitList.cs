using Multiplify.Domain.Common;

namespace Multiplify.Domain;
public class WaitList : AuditableEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

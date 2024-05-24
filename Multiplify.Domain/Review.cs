using Multiplify.Domain.Common;
using Multiplify.Domain.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Multiplify.Domain;
public class Review : AuditableEntity
{
    public int Id { get; set; }
    public string EntreprenuerId { get; set; } = string.Empty;
    public int Rate { get; set; }
    public string? Comment { get; set; }

    public string ReviewerId { get; set; } = string.Empty;
    [ForeignKey("ReviewerId")]
    public AppUser Reviewer { get; set; }


    [ForeignKey("EntreprenuerId")]
    public AppUser Entreprenuer { get; set; }
}

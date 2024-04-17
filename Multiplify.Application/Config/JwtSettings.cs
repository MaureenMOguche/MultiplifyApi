using System.ComponentModel.DataAnnotations;

namespace Multiplify.Application.Config;
public class JwtSettings
{
    [Required]
    public required string Issuer { get; set; }
    [Required]
    public required string Audience { get; set; }
    [Required]
    public required string Key { get; set; }
    [Required]
    public required int DurationInMinutes { get; set; }
}

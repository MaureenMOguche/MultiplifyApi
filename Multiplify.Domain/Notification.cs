using Multiplify.Domain.Enums;

namespace Multiplify.Domain;
public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
    public string? Link { get; set; }
    public string? ImageUrl { get; set; }
    public string? Title { get; set; }
    public NotificationType NotificationType { get; set; }
}

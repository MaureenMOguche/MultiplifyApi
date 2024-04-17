using System.ComponentModel.DataAnnotations;

namespace Multiplify.Application.Models;
public class EmailMessage
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Inavalid email")]
    public string To { get; set; } = string.Empty;
    [Required(ErrorMessage = "Subject is required")]
    public string Subject { get; set; } = string.Empty;
    [Required(ErrorMessage = "Body is required")]
    public string Body { get; set; } = string.Empty;
}

public class EmailMessageForWaitlist
{
    [Required(ErrorMessage = "Subject is required")]
    public string Subject { get; set; } = string.Empty;
    public string? Title { get; set; }
    [Required(ErrorMessage = "Message is required")]
    public string Message { get; set; } = string.Empty;
}

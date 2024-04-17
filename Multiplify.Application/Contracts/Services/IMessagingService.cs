using Multiplify.Application.Models;

namespace Multiplify.Application.Contracts.Services;
public interface IMessagingService
{
    //Iron pdf or Questpdf
    //Task<bool> SendEmailAsync(EmailMessage emailMessage, PdfDocument? document = null, string? filename = null);
    Task<bool> SendEmailAsync(EmailMessage emailMessage);

}

using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Models;

namespace Multiplify.Application.ServiceImplementations;
public class MessagingService(IOptions<EmailSettings> emailSettings) : IMessagingService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;
    public Task<bool> SendEmailAsync(EmailMessage emailMessage)
    {
        var emailToSend = new MimeMessage();
        emailToSend.From.Add(MailboxAddress.Parse(_emailSettings.Email));
        emailToSend.To.Add(MailboxAddress.Parse(emailMessage.To));
        emailToSend.Subject = emailMessage.Subject;
        var bodyBuilder = new BodyBuilder();

        bodyBuilder.HtmlBody = emailMessage.Body;
        emailToSend.Body = bodyBuilder.ToMessageBody();

        //send email
        using (var smtp = new SmtpClient())
        {
            smtp.Connect(_emailSettings.SmtpServer, _emailSettings.Port, true);
            smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
            smtp.Send(emailToSend);
            smtp.Disconnect(true);
        }

        return Task.FromResult(true);
    }

    //public Task<bool> SendEmailAsync(EmailMessage emailMessage, PdfDocument? document = null, string? filename = null)
    //{
    //    var emailToSend = new MimeMessage();
    //    emailToSend.From.Add(MailboxAddress.Parse(_emailSettings.Email));
    //    emailToSend.To.Add(MailboxAddress.Parse(emailMessage.To));
    //    emailToSend.Subject = emailMessage.Subject;
    //    var bodyBuilder = new BodyBuilder();


    //    //Adding Attachments
    //    if (document != null)
    //    {
    //        bodyBuilder.Attachments.Add(filename,
    //            document.Stream.ToArray(), ContentType.Parse("application/octet-stream"));
    //    }

    //    bodyBuilder.HtmlBody = emailMessage.Body;
    //    emailToSend.Body = bodyBuilder.ToMessageBody();

    //    //send email
    //    using (var smtp = new SmtpClient())
    //    {
    //        smtp.Connect(_emailSettings.SmtpServer, _emailSettings.Port, true);
    //        smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
    //        smtp.Send(emailToSend);
    //        smtp.Disconnect(true);
    //    }

    //    return Task.FromResult(true);
    //}
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos.Waitlist;
using Multiplify.Application.Models;
using Multiplify.Domain;
using ReenUtility.Responses;

namespace Multiplify.Application.ServiceImplementations;
public class WaitlistService(IUnitOfWork db, IMessagingService messagingService) : IWaitlistService
{
    public async Task<ApiResponse> JoinWaitList(JoinWaitlistDto joinWaitlistDto)
    {
        var emailExists = await db.GetRepository<WaitList>()
            .GetAsync(x => x.Email.ToLower().Equals(joinWaitlistDto.Email.ToLower()))
            .FirstOrDefaultAsync();

        if (emailExists != null)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "Email already exists");

        var waitListPerson = new WaitList
        {
            Email = joinWaitlistDto.Email,
            FullName = joinWaitlistDto.FullName,
        };

        await db.GetRepository<WaitList>().AddAsync(waitListPerson);

        if (await db.SaveChangesAsync())
        {
            await messagingService.SendEmailAsync(new EmailMessage
            {
                To = joinWaitlistDto.Email,
                Subject = "Welcome to Multiplify",
                Body = EmailTemplates.WaitlistEmail(joinWaitlistDto.FullName)
            });
            return ApiResponse.Success("Successfully joined waitlist");
        }

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to join waitlist");
    }

    public async Task<ApiResponse> SendSingleEmail(EmailMessage emailMessage)
    {
        if(await messagingService.SendEmailAsync(emailMessage))
            return ApiResponse.Success("Email sent successfully");
        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to send email");
    }

    public async Task<ApiResponse> SendToAllWaitlist(EmailMessageForWaitlist emailMessage)
    {
        var allWaitlist = await db.GetRepository<WaitList>()
            .GetAsync().ToListAsync();

        
        foreach (var user in allWaitlist)
        {
            var emailContent = new EmailMessage
            {
                To = user.Email,
                Subject = emailMessage.Subject,
                Body = EmailTemplates.Prefix(user.FullName,
                emailMessage.Title!, emailMessage.Message)
            };

           await messagingService.SendEmailAsync(emailContent);
        }

        return ApiResponse.Success("Email sent successfully");
    }
}

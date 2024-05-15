using Multiplify.Application.Dtos.Waitlist;
using Multiplify.Application.Models;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Contracts.Services;
public interface IWaitlistService
{
    Task<ApiResponse> JoinWaitList(JoinWaitlistDto joinWaitlistDto);
    Task<ApiResponse> SendSingleEmail(EmailMessage emailMessage);
    Task<ApiResponse> SendToAllWaitlist(EmailMessageForWaitlist emailMessage);
}

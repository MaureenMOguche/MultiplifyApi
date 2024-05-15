using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos.Waitlist;
using Multiplify.Application.Models;
using Multiplify.Application.Responses;

namespace Multiplify.Api.Controllers;

/// <summary>
/// Waitlist controller
/// </summary>
/// <param name="waitlistService"></param>
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class WaitlistController(IWaitlistService waitlistService) : ControllerBase
{
    /// <summary>
    /// Allows a user join the multiplify waitlist
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [HttpPost("join-waitlist")]
    public async Task<IActionResult> JoinWaitlist(JoinWaitlistDto joinWaitlist)
    {
        var result = await waitlistService.JoinWaitList(joinWaitlist);
        return StatusCode(result.StatusCode, result);
    }


    /// <summary>
    /// sends an email to a single person
    /// </summary>
    /// <param name="emailMessage"></param>
    /// <returns></returns>
    [HttpPost("send-single-email")]
    public async Task<IActionResult> SendSingleEmail(EmailMessage emailMessage)
    {
        var result = await waitlistService.SendSingleEmail(emailMessage);
        return StatusCode(result.StatusCode, result);
    }


    /// <summary>
    /// Sends an email to all the people on the waitlist
    /// </summary>
    /// <param name="emailMessage"></param>
    /// <returns></returns>
    [HttpPost("send-bulk-email")]
    public async Task<IActionResult> SendToAllWaitList(EmailMessageForWaitlist emailMessage)
    {
        var result = await waitlistService.SendToAllWaitlist(emailMessage);
        return StatusCode(result.StatusCode, result);
    }
}

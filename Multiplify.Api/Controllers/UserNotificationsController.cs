using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos;
using Multiplify.Application.Middlewares;
using Multiplify.Application.Responses;

namespace Multiplify.Api.Controllers;

/// <summary>
/// User notifications controller
/// </summary>
/// <param name="userNotificationService"></param>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserNotificationsController(IUserNotificationService userNotificationService) : ControllerBase
{
    /// <summary>
    /// Gets a users notifications
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] BaseQueryParams queryParams)
    {
        var response = await userNotificationService.GetNotifications(queryParams);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Gets a single notification
    /// </summary>
    /// <param name="notificationId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpGet("{notificationId}")]
    public async Task<IActionResult> GetNotification(int notificationId)
    {
        var response = await userNotificationService.GetNotification(notificationId);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Deletes a single notification
    /// </summary>
    /// <param name="notificationId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        var response = await userNotificationService.DeleteNotification(notificationId);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Marks a notification as read
    /// </summary>
    /// <param name="notificationId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("{notificationId}")]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        var response = await userNotificationService.MarkNotificationAsRead(notificationId);
        return StatusCode(response.StatusCode, response);
    }
}

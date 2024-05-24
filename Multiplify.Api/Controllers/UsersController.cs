using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos.User;

namespace Multiplify.Api.Controllers;


/// <summary>
/// Users controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Gets a user's profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfile(string userId)
    {
        var response = await userService.GetProfile(userId);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Updates a user's profile
    /// </summary>
    /// <param name="profileId"></param>
    /// <param name="updateProfile"></param>
    /// <returns></returns>
    [HttpPut("{profileId}")]
    public async Task<IActionResult> UpdateProfile(string profileId, [FromForm]UpdateProfileDto updateProfile)
    {
        var response = await userService.UpdateProfile(profileId, updateProfile);
        return StatusCode(response.StatusCode, response);
    }
}

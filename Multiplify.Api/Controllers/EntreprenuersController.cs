using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos;
using Multiplify.Application.Middlewares;
using Multiplify.Application.Responses;

namespace Multiplify.Api.Controllers;


/// <summary>
/// Entreprenuers controller    
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EntreprenuersController(IEntreprenuerService entreprenuerService) : ControllerBase
{

    /// <summary>
    /// Create a service profile
    /// </summary>
    /// <param name="createServiceProfileDto"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("create-service-profile")]
    public async Task<IActionResult> CreateServiceProfile([FromForm]CreateServiceProfileDto createServiceProfileDto)
    {
        var response = await entreprenuerService.CreateServiceProfile(createServiceProfileDto);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get business profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpGet("business-profile/{userId}")]
    public async Task<IActionResult> GetBusinessProfile(string userId)
    {
        var response = await entreprenuerService.GetBusinessProfile(userId);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Get service profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpGet("service-profile/{userId}")]
    public async Task<IActionResult> GetServiceProfile(string userId)
    {
        var response = await entreprenuerService.GetServiceProfile(userId);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Update business profile
    /// </summary>
    /// <param name="updateBusinessProfile"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut("update-business-profile")]
    public async Task<IActionResult> UpdateBusinessProfile(UpdateBusinessProfileDto updateBusinessProfile)
    {
        var response = await entreprenuerService.UpdateBusinessProfile(updateBusinessProfile);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update service profile
    /// </summary>
    /// <param name="updateServiceProfileDto"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut("update-service-profile")]
    public async Task<IActionResult> UpdateServiceProfile(UpdateServiceProfileDto updateServiceProfileDto)
    {
        var response = await entreprenuerService.UpdateServiceProfile(updateServiceProfileDto);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Apply for funding
    /// </summary>
    /// <param name="applyForFunding"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("apply-for-funding")]
    public async Task<IActionResult> ApplyForFunding(ApplyForFundingDto applyForFunding)
    {
        var response = await entreprenuerService.ApplyForFunding(applyForFunding);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Gets all the avaiable fundings
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("all-fundings")]
    public async Task<IActionResult> GetAllFundings([FromQuery]BaseQueryParams queryParams)
    {
        var response = await entreprenuerService.GetAllFundings(queryParams);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Gets all fundings where funding category matches user business category or industry
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("my-applications")]
    public async Task<IActionResult> MyApplications([FromQuery]BaseQueryParams queryParams)
    {
        var response = await entreprenuerService.MyApplications(queryParams);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Gets service profiles of all entreprenuers for the market place
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("service-profiles")]
    public async Task<IActionResult> GetAllServiceProfiles([FromQuery]BaseQueryParams queryParams)
    {
        var response = await entreprenuerService.GetAllServiceProfiles(queryParams);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Gets all reviews of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("user-reviews/{userId}")]
    public async Task<IActionResult> GetUserReviews(string userId)
    {
        var response = await entreprenuerService.GetUserReviews(userId);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Gets recommended fundings for the user based on user business category or industry
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("user-recommended-funding")]
    public async Task<IActionResult> UserRecommendedFunding([FromQuery]BaseQueryParams queryParams)
    {
        var response = await entreprenuerService.UserRecommendedFunding(queryParams);
        return StatusCode(response.StatusCode, response);
    }

}

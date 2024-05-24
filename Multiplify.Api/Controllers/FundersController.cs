using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos;
using Multiplify.Application.Middlewares;
using Multiplify.Application.Responses;

namespace Multiplify.Api.Controllers;

/// <summary>
/// Funders controller
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FundersController(IFunderService funderService) : ControllerBase
{
    /// <summary>
    /// Allows a fund provider to add a fund
    /// </summary>
    /// <param name="fundDto"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpPost("add-fund")]
    public async Task<IActionResult> AddFund([FromForm] AddFundDto fundDto)
    {
        var response = await funderService.AddFund(fundDto);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Allows a fund provider to approve or reject an application for a fund
    /// </summary>
    /// <param name="approve"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpPost("approve-or-reject-application")]
    public async Task<IActionResult> ApproveOrRejectApplication([FromBody] ApproveOrRejectApplicationDto approve)
    {
        var response = await funderService.ApproveOrRejectApplication(approve);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Allows a fund provider to get all funds they have added
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("all-funds")]
    public async Task<IActionResult> GetAllFunds([FromQuery] BaseQueryParams queryParams)
    {
        var response = await funderService.GetAllFunds(queryParams);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Allows a fund provider to get all applications for a fund
    /// </summary>
    /// <param name="queryParams"></param>
    /// <param name="fundId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("all-fund-applications/{fundId}")]
    public async Task<IActionResult> GetAllFundApplications(int fundId, [FromQuery] BaseQueryParams queryParams)
    {
        var response = await funderService.GetFundApplications(fundId, queryParams);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Allows a fund provider to delete a specific fund
    /// </summary>
    /// <param name="fundId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpDelete("remove-fund/{fundId}")]
    public async Task<IActionResult> RemoveFund(int fundId)
    {
        var response = await funderService.RemoveFund(fundId);
        return StatusCode(response.StatusCode, response);
    }
}

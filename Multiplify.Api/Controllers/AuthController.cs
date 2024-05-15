using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos.User;
using Multiplify.Application.Responses;
using Multiplify.Domain;

namespace Multiplify.Api.Controllers;

/// <summary>
/// Auth controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registrationRequest"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
    {
        var response = await authService.Register(registrationRequest);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Confirms a user email
    /// </summary>
    /// <param name="confirmEmail"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> Register(ConfirmEmailRequest confirmEmail)
    {
        var response = await authService.ConfirmEmail(confirmEmail);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Confirms a user email
    /// </summary>
    /// <param name="resendConfirmation"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("resend-email-confirmation")]
    public async Task<IActionResult> ResendEmailConfirmation(ForgotPasswordRequest resendConfirmation)
    {
        var response = await authService.ResendConfirmationEmail(resendConfirmation.Email);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var response = await authService.Login(loginRequest);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Allows a user request a password reset
    /// </summary>
    /// <param name="forgotPassword"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPassword)
    {
        var response = await authService.ForgotPassword(forgotPassword.Email);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Allows a user reset password
    /// </summary>
    /// <param name="resetPassword"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("password-reset")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPassword)
    {
        var response = await authService.ResetPassword(resetPassword);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Allows a user change password
    /// </summary>
    /// <param name="changePassword"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePassword)
    {
        var response = await authService.ChangePassword(changePassword);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Completes registration for entreprenuer
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="businessInformation"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("entreprenuer-complete/{userId}")]
    public async Task<IActionResult> CompleteEntreprenuer(string userId, EntreprenuerCompleteRegistration businessInformation)
    {
        var response = await authService.EntreprenuerCompleteRegistration(userId, businessInformation);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Completes registration for fund provider
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fundInterests"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("fund-provider-complete/{userId}")]
    public async Task<IActionResult> CompleteFundProvider(string userId, List<string> fundInterests)
    {
        var response = await authService.FunderCompleteRegistration(userId, fundInterests);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Completes registration for market explorer
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="explorationInterests"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("market-explorer-complete/{userId}")]
    public async Task<IActionResult> CompleteMarketExplorer(string userId, List<string> explorationInterests)
    {
        var response = await authService.MarketExplorerCompleteRegistration(userId, explorationInterests);
        return StatusCode(response.StatusCode, response);
    }

}

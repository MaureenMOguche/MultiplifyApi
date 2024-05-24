using Azure.Core;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Multiplify.Application.Config;
using Multiplify.Application.Constants;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos.User;
using Multiplify.Application.Models;
using Multiplify.Application.Responses;
using Multiplify.Application.ServiceImplementations.Helpers;
using Multiplify.Domain;
using Multiplify.Domain.Enums;
using Multiplify.Domain.User;
using Org.BouncyCastle.Asn1.Cmp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Multiplify.Application.ServiceImplementations;
public class AuthService(IUnitOfWork db,
    IMessagingService messagingService,
    IConfiguration config,
    IOptions<JwtSettings> jwtSettings,
    IMemoryCache memoryCache,
    IPhotoService photoService) : IAuthService
{
    
    private readonly string mulitplifyUrl = config.GetValue<string>("MultiplifyUrl")!;
    private readonly string mulitplifyTempUrl = config.GetValue<string>("MultiplifyTempUrl")!;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<ApiResponse> ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        var currentUser = UserHelper.CurrentUser();
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User not authorized");

        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Email == currentUser.Email, true).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Email not found");

        var isPasswordValid = BC.EnhancedVerify(changePasswordRequest.CurrentPassword, user.PasswordHash);
        if (!isPasswordValid)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "Invalid password");

        user.PasswordHash = BC.EnhancedHashPassword(changePasswordRequest.NewPassword);

        db.GetRepository<AppUser>().Update(user);
        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully changed password");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "An error occured while changing password, please try again");
    }

    public async Task<ApiResponse> ConfirmEmail(ConfirmEmailRequest confirmEmail)
    {
        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Email == confirmEmail.Email, true).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Email not found");

        var isTokenValid = TokenProviders.ValidateEmailCodeToken(TokenPurpose.ConfirmEmail.ToString(), confirmEmail.Token, confirmEmail.Email);

        if (!isTokenValid)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "Invalid token, try again");

        user.EmailConfirmed = true;

        db.GetRepository<AppUser>().Update(user);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully confirmed email");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "An error occured when confirming the email, please try again");
    }

    public async Task<ApiResponse> EntreprenuerCompleteRegistration(EntreprenuerCompleteRegistration completeRegistration)
    {
        var currentUser = UserHelper.CurrentUser();
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User not authorized");

        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Id == currentUser.Id).FirstOrDefaultAsync();
        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "User not found");

        var userBusiness = await db.GetRepository<BusinessInformation>().EntityExists(x => x.EntreprenuerId == user.Id);

        if (user.IsRoleSet && userBusiness)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "You have already completed your onboarding");

        user.IsRoleSet = true;
        user.Role = Roles.Entreprenuer.ToString();
        currentUser.Role = Roles.Entreprenuer.ToString();

        db.GetRepository<AppUser>().Update(user);

        List<string> certs = new();
        if (completeRegistration.Certifications != null && completeRegistration.Certifications.Count > 0)
        {
            for (int cert = 0; cert < completeRegistration.Certifications.Count; cert++)
            {
                var imageRes = await photoService.AddPhotoAsync(completeRegistration.Certifications[cert], 
                    $"{user.UserName}_certification{cert}", $"MultiplifyCerts");

                if (imageRes.StatusCode == System.Net.HttpStatusCode.OK)
                   certs.Add(imageRes.SecureUrl.AbsoluteUri);
            }
        }

        var business = new BusinessInformation
        {
            Name = completeRegistration.BusinessName,
            Description = completeRegistration.BusinessDescription ?? string.Empty,
            Stage = completeRegistration.BusinessStage,
            Industry = completeRegistration.Industry,
            AverageIncome = completeRegistration.AverageIncome,
            Entreprenuer = user,
            Certifications = certs.Count <= 0 ? string.Empty : certs.Count == 1 ? certs[0] : string.Join(", ", certs),
        };

        await db.GetRepository<BusinessInformation>().AddAsync(business);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Onboarding completed successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to complete onboarding");
    }

    public async Task<ApiResponse> ForgotPassword(string email)
    {
        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Email == email, true).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Email not found");

        var token = TokenProviders.GeneratePasswordResetToken(TokenPurpose.PasswordReset.ToString(), user.Id!);

        var returnUrl = $"{mulitplifyTempUrl}/reset-password?token={token}";

        var emailMessage = new EmailMessage
        {
            To = user.Email!,
            Subject = "Multiplify - Reset Password Request",
            Body = EmailTemplates.ResetPasswordEmail(user.FullName, returnUrl)
        };

        BackgroundJob.Enqueue(() => messagingService.SendEmailAsync(emailMessage));

        return ApiResponse.Success("Successfully sent reset link");
    }

    public async Task<ApiResponse> Login(LoginRequest loginRequest)
    {
        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Email == loginRequest.Email, true).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Invalid credentials");

        var validatePassword = BC.EnhancedVerify(loginRequest.Password, user.PasswordHash);
        if (!validatePassword)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Invalid credentials");

        (string accesstoken, string refreshtoken) = GenerateLoginToken(user);

        user.RefreshToken  = refreshtoken;
        user.RefreshExpiry = DateTime.UtcNow.AddDays(2);

        db.GetRepository<AppUser>().Update(user);
        if (await db.SaveChangesAsync())
            return ApiResponse<object>.Success(new 
            {
                Username = user.UserName,
                UserId = user.Id,
                user.IsRoleSet,
                user.Role,
                Token = new
                {
                    AccessToken = accesstoken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                    RefreshToken = refreshtoken,
                    RefreshTokenExpiry = user.RefreshExpiry
                }

            }, "Authentication successfull");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Authentication failed, please try again");
    }

    private (string accesstoken, string refreshtoken) GenerateLoginToken(AppUser user)
    {
        var role = user.Role ?? "";

        var claims = new Claim[]
        {
            new(ClaimTypes.Role, role),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
        signingCredentials: signingCredentials,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)
            );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = $"{Guid.NewGuid()}_{Guid.NewGuid()}_{DateTime.UtcNow}_{Guid.NewGuid()}_{Guid.NewGuid()}";

        return (tokenString, refreshToken);
    }

    public async Task<ApiResponse> Register(RegistrationRequest registrationRequest)
    {
        var userExists = await db.GetRepository<AppUser>().GetAsync(x => x.Email == registrationRequest.Email).FirstOrDefaultAsync();
        if (userExists != null)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "User already exists");

        var user = new AppUser
        {
            FirstName = registrationRequest.FirstName,
            LastName = registrationRequest.LastName,
            Email = registrationRequest.Email,
            UserName = registrationRequest.Email,
            PasswordHash = BC.EnhancedHashPassword(registrationRequest.Password)
        };

        var passwordList = new List<string> { BC.EnhancedHashPassword(registrationRequest.Password) };
        memoryCache.Set(user.Id, passwordList);

        await db.GetRepository<AppUser>().AddAsync(user);

        if (!await db.SaveChangesAsync())
            return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to register user");

        (var token, string refreshToken) = GenerateLoginToken(user);


        var emailMessage = new EmailMessage
        {
            To = registrationRequest.Email,
            Subject = "Welcome to Multiplify",
            Body = $"Dear {user.FullName}, welcome to multiplify!"
        };

        BackgroundJob.Enqueue(() => messagingService.SendEmailAsync(emailMessage));

        return ApiResponse<object>.Success(new 
        {
            UserId = user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.IsRoleSet,
            AccessToken = token
        }, "User registered successfully");
    }
    
    public async Task<ApiResponse> RegisterWithConfirmation(RegistrationRequest registrationRequest)
    {
        var userExists = await db.GetRepository<AppUser>().GetAsync(x => x.Email == registrationRequest.Email).FirstOrDefaultAsync();
        if (userExists != null)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "User already exists");

        var user = new AppUser
        {
            FirstName = registrationRequest.FirstName,
            LastName = registrationRequest.LastName,
            Email = registrationRequest.Email,
            UserName = registrationRequest.Email,
            PasswordHash = BC.EnhancedHashPassword(registrationRequest.Password)
        };

        await db.GetRepository<AppUser>().AddAsync(user);

        if (!await db.SaveChangesAsync())
            return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to register user");

        var token = TokenProviders.GenerateEmailCodeToken(TokenPurpose.ConfirmEmail.ToString(), user.Email);

        var returnUrl = $"{mulitplifyUrl}/confirm-email?email={user.Email}&token={token}";

        var emailMessage = new EmailMessage
        {
            To = registrationRequest.Email,
            Subject = "Welcome to Multiplify",
            Body = $"Welcome, {user.FullName}, please click the link to verify your email {returnUrl}"
        };

        BackgroundJob.Enqueue(() => messagingService.SendEmailAsync(emailMessage));

        return ApiResponse.Success("User registered successfully");
    }

    public async Task<ApiResponse> ResendConfirmationEmail(string email)
    {
        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Email == email, true).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Email not found");

        var token = TokenProviders.GenerateEmailCodeToken(TokenPurpose.ConfirmEmail.ToString(), user.Email!);

        var returnUrl = $"{mulitplifyUrl}/confirm-email?email={user.Email}&token={token}";

        var emailMessage = new EmailMessage
        {
            To = email,
            Subject = "Welcome to Multiplify",
            Body = $"Welcome, {user.FullName}, please click the link to verify your email {returnUrl}"
        };

        BackgroundJob.Enqueue(() => messagingService.SendEmailAsync(emailMessage));

        return ApiResponse<AppUser>.Success(user, "Successfully resent email confirmation");
    }

    public async Task<ApiResponse> ResetPassword(ResetPasswordRequest resetPasswordRequest)
    {
        (bool isTokenValid, string message) = await TokenProviders.ValidatePasswordResetToken(TokenPurpose.PasswordReset.ToString(), resetPasswordRequest.Token, resetPasswordRequest.Password);

        if (!isTokenValid)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, message);

        return ApiResponse.Success("Successfully reset password");

    }

    public async Task<ApiResponse> FunderCompleteRegistration(FunderBusinessInterests businessInterests)
    {
        var currentUser = UserHelper.CurrentUser();
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User not authorized");

        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Id == currentUser.Id).FirstOrDefaultAsync();
        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "User not found");

        user.IsRoleSet = true;
        user.Role = Roles.FundProvider.ToString();
        user.FundProviderInterests = string.Join(", ", businessInterests.BusinessInterets);
        currentUser.Role = Roles.FundProvider.ToString();

        db.GetRepository<AppUser>().Update(user);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully completed funder profile");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "An error occured while completing funder profile, please try again");
    }

    public async Task<ApiResponse> MarketExplorerCompleteRegistration(ExplorerInterests exploreInterests)
    {
        var currentUser = UserHelper.CurrentUser();
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User not authorized");

        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Id == currentUser.Id).FirstOrDefaultAsync();
        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "User not found");

        user.IsRoleSet = true;
        user.Role = Roles.MarketExplorer.ToString();
        currentUser.Role = Roles.MarketExplorer.ToString();

        if (exploreInterests != null)
            user.MarketExplorerInterests = string.Join(", ", exploreInterests);

        db.GetRepository<AppUser>().Update(user);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully completed onboarding as market explorer");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "An error occured while completing explorer profile, please try again");
    }

    public async Task<ApiResponse> RefreshToken()
    {
        var currentUser = UserHelper.CurrentUser();
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User not authorized");

        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Id == currentUser.Id).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "User not found");

        if (user.RefreshExpiry < DateTime.UtcNow)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "Refresh token has expired, please login again");

        (string accesstoken, string refreshtoken) = GenerateLoginToken(user);

        user.RefreshToken = refreshtoken;
        user.RefreshExpiry = DateTime.UtcNow.AddDays(2);

        db.GetRepository<AppUser>().Update(user);

        if (await db.SaveChangesAsync())
            return ApiResponse<object>.Success(new
            {
                Username = user.UserName,
                user.Role,
                Token = new
                {
                    AccessToken = accesstoken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                    RefreshToken = refreshtoken,
                    RefreshTokenExpiry = user.RefreshExpiry
                }

            }, "Token refreshed successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to refresh token, please try again");
    }
}

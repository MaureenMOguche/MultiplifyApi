using Multiplify.Application.Dtos.User;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Contracts.Services;
public interface IAuthService
{
    Task<ApiResponse> Register(RegistrationRequest registrationRequest);
    Task<ApiResponse> EntreprenuerCompleteRegistration(string userId, EntreprenuerCompleteRegistration completeRegistration);
    Task<ApiResponse> FunderCompleteRegistration(string userId, List<string> businessInterests);
    Task<ApiResponse> MarketExplorerCompleteRegistration(string userId, List<string> ExploreInterest);
    Task<ApiResponse> Login(LoginRequest loginRequest);
    Task<ApiResponse> ConfirmEmail(ConfirmEmailRequest confirmEmail);
    Task<ApiResponse> ResendConfirmationEmail(string email);
    Task<ApiResponse> ForgotPassword(string email);
    Task<ApiResponse> ResetPassword(ResetPasswordRequest resetPasswordRequest);
    Task<ApiResponse> ChangePassword(ChangePasswordRequest changePasswordRequest);
    //Task<ApiResponse> UpdateProfile(UpdateProfileRequest updateProfileRequest);
}

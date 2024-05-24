using Multiplify.Application.Dtos.User;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Contracts.Services;
public interface IAuthService
{
    Task<ApiResponse> Register(RegistrationRequest registrationRequest);
    Task<ApiResponse> RegisterWithConfirmation(RegistrationRequest registrationRequest);
    Task<ApiResponse> EntreprenuerCompleteRegistration(EntreprenuerCompleteRegistration completeRegistration);
    Task<ApiResponse> FunderCompleteRegistration(FunderBusinessInterests businessInterests);
    Task<ApiResponse> MarketExplorerCompleteRegistration(ExplorerInterests ExploreInterest);
    Task<ApiResponse> Login(LoginRequest loginRequest);
    Task<ApiResponse> ConfirmEmail(ConfirmEmailRequest confirmEmail);
    Task<ApiResponse> ResendConfirmationEmail(string email);
    Task<ApiResponse> ForgotPassword(string email);
    Task<ApiResponse> ResetPassword(ResetPasswordRequest resetPasswordRequest);
    Task<ApiResponse> ChangePassword(ChangePasswordRequest changePasswordRequest);
    Task<ApiResponse> RefreshToken();
    //Task<ApiResponse> UpdateProfile(UpdateProfileRequest updateProfileRequest);
}

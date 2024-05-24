using Microsoft.AspNetCore.Http;
using Multiplify.Application.Dtos.User;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Contracts.Services;
public interface IUserService
{
    Task<ApiResponse> UpdateProfile(string profileId, UpdateProfileDto updateProfile);
    Task<ApiResponse> GetProfile(string? userId);
    Task<ApiResponse> UpdateProfileImage(IFormFile image);
    Task<ApiResponse> UpdateProfileCoverImage(IFormFile image);

}

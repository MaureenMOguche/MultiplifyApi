using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos.User;
using Multiplify.Application.Responses;
using Multiplify.Domain.User;

namespace Multiplify.Application.ServiceImplementations;
public class UserService(IUnitOfWork db,
    IPhotoService photoService) : IUserService
{
    public async Task<ApiResponse> GetProfile(string? userId)
    {
        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Id == userId).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "User not found");

        var userDto = new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.UserName,
            user.ProfilePicture,
            user.Biography
        };

        return ApiResponse<object>.Success(userDto, "Successfully retrieved user profile");
    }

    public async Task<ApiResponse> UpdateProfile(string profileId, UpdateProfileDto updateProfile)
    {
        var user = await db.GetRepository<AppUser>().GetAsync(x => x.Id == profileId).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "User not found");

        user.FirstName = updateProfile.FirsName ?? user.FirstName;
        user.LastName = updateProfile.LastName ?? user.LastName;

        if (updateProfile.ProfilePicture != null && updateProfile.ProfilePicture.Length >0)
        {
            var res = await photoService.AddPhotoAsync(updateProfile.ProfilePicture, $"{user.FullName.Replace(" ", "_")}", "Multiplify_Profiles");

            if (res.StatusCode != System.Net.HttpStatusCode.OK)
                return ApiResponse.Failure((int)res.StatusCode, res.Error.Message);

            user.ProfilePicture = res.SecureUrl.AbsoluteUri;
        }

        user.Biography = updateProfile.Biography ?? user.Biography;

        db.GetRepository<AppUser>().Update(user);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Profile updated successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "An error occurred while updating profile");
    }

    public Task<ApiResponse> UpdateProfileCoverImage(IFormFile image)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> UpdateProfileImage(IFormFile image)
    {
        throw new NotImplementedException();
    }
}

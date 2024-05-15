using Multiplify.Application.Dtos;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Contracts.Services;
public interface IEntreprenuerService
{
    Task<ApiResponse> UpdateBusinessProfile(UpdateBusinessProfileDto updateBusinessProfile);
    Task<ApiResponse> GetBusinessProfile(string userId);
    Task<ApiResponse> GetServiceProfile(string userId);
    Task<ApiResponse> UpdateServiceProfile(UpdateServiceProfileDto updateServiceProfileDto);
    Task<ApiResponse> ApplyForFunding(ApplyForFundingDto applyForFunding);
    Task<ApiResponse> GetAllFundings(BaseQueryParams queryParams);
    Task<ApiResponse> MyApplications(BaseQueryParams queryParams);
    Task<ApiResponse> GetAllServiceProfiles(BaseQueryParams queryParams);
    Task<ApiResponse> GetUserReviews(string userId);
    Task<ApiResponse> UserRecommendedFunding();
    Task<ApiResponse> AddUserReview(string userId, AddUserReviewDto addUserReview);
}

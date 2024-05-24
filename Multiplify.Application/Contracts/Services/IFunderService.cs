using Multiplify.Application.Dtos;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Contracts.Services;
public interface IFunderService
{
    Task<ApiResponse> AddFund(AddFundDto addFund);
    Task<ApiResponse> RemoveFund(int fundId);
    Task<ApiResponse> UpdateFund(int fundId);
    Task<ApiResponse> GetAllFunds(BaseQueryParams queryParams);
    Task<ApiResponse> GetFund(int fundId);
    Task<ApiResponse> GetFundApplications(int fundId, BaseQueryParams queryParams);
    Task<ApiResponse> GetAllApplications(BaseQueryParams queryParams);
    Task<ApiResponse> ApproveOrRejectApplication(ApproveOrRejectApplicationDto approve);
}

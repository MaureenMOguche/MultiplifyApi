using Microsoft.AspNetCore.Http;
using Multiplify.Application.Constants;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos;
using Multiplify.Application.Responses;
using Multiplify.Application.ServiceImplementations.Helpers;
using Multiplify.Domain;

namespace Multiplify.Application.ServiceImplementations;
public class FunderService(IUnitOfWork db,
    IPhotoService photoService) : IFunderService
{
    private readonly UserPrincipal currentUser = UserHelper.CurrentUser();
    public async Task<ApiResponse> AddFund(AddFundDto addFund)
    {
        if (!currentUser.IsInRole(ApplicationRoles.FundProvider))
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "Action not allowed: only fund providers can add fund");

        var fundExists = await db.GetRepository<Fund>().EntityExists(f => f.Name == addFund.Name);

        if (fundExists)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "Fund already exists");

        var fund = new Fund
        {
            Name = addFund.Name,
            Description = addFund.Description,
            MininumRange = addFund.MininumRange,
            MaximumRange = addFund.MaximumRange,
            ApplicationDeadline = addFund.ApplicationDeadline,
            Categories = string.Join(", ", addFund.Categories),
            Requirements = addFund.Requirements,
        };

        if (addFund.Image != null && addFund.Image.Length > 0)
        {
            var result = await photoService.AddPhotoAsync(addFund.Image, addFund.Name.Replace(" ", "_"), "Multiplify/Funds");
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                fund.ImageUrl = result.Url.AbsoluteUri;
        }

        await db.GetRepository<Fund>().AddAsync(fund);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully added fund");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to add fund");
        
    }

    public Task<ApiResponse> ApproveApplication(int applicationId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> GetAllApplications(BaseQueryParams queryParams)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> GetAllFunds(BaseQueryParams queryParams)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> GetFund(int fundId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> GetFundApplications(int fundId, BaseQueryParams queryParams)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> RemoveFund(int fundId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> UpdateFund(int fundId)
    {
        throw new NotImplementedException();
    }
}

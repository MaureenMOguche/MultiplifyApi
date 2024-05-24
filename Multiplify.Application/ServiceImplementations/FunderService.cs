using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Multiplify.Application.Constants;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos;
using Multiplify.Application.Responses;
using Multiplify.Application.ServiceImplementations.Helpers;
using Multiplify.Domain;
using Multiplify.Domain.Enums;

namespace Multiplify.Application.ServiceImplementations;
public class FunderService(IUnitOfWork db,
    IPhotoService photoService) : IFunderService
{
    private readonly UserPrincipal? currentUser = UserHelper.CurrentUser();
    public async Task<ApiResponse> AddFund(AddFundDto addFund)
    {
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized");

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
            FunderId = currentUser.Id
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

    public async Task<ApiResponse> ApproveOrRejectApplication(ApproveOrRejectApplicationDto approve)
    {
        var application = await db.GetRepository<FundApplication>().GetAsync(x => x.Id == approve.ApplicationId).FirstOrDefaultAsync();

        if (application == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Application not found");
        Notification? notification = null;
        if (approve.Approve)
        {
            application.Status = FundApplicationStatus.Approved;

            notification = new Notification
            {
                Title = "Application Approved",
                Message = $"Your application for {application.Fund.Name} has been approved",
                Comment = approve.Reason ?? "",
                NotificationType = NotificationType.FundApplicationApproved,
                ImageUrl = application.Fund.ImageUrl,
                Link = $"/funds/{application.Fund.Id}",
                UserId = application.ApplicantId
            };
        }
        else
        {
            application.Status = FundApplicationStatus.Rejected;

            notification = new Notification
            {
                Title = "Application Rejected",
                Message = $"Your application for {application.Fund.Name} has been rejected",
                Comment = approve.Reason ?? "",
                NotificationType = NotificationType.FundApplicationRejected,
                ImageUrl = application.Fund.ImageUrl,
                Link = $"/funds/{application.Fund.Id}",
                UserId = application.ApplicantId
            };
        }
        

        await db.GetRepository<Notification>().AddAsync(notification);
        db.GetRepository<FundApplication>().Update(application);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully approved application");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to approve application");

    }

    public Task<ApiResponse> GetAllApplications(BaseQueryParams queryParams)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse> GetAllFunds(BaseQueryParams queryParams)
    {
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized");

        if (!currentUser.IsInRole(ApplicationRoles.FundProvider))
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "Action not allowed: only fund providers can view funds");

        IQueryable<Fund> funds = db.GetRepository<Fund>().GetAsync(x => x.FunderId == currentUser.Id)
            .Include(x => x.Applicants)
            .ThenInclude(x => x.Applicant);

        if (!string.IsNullOrEmpty(queryParams.Search))
            funds = funds.Where(f => f.Name.Contains(queryParams.Search) || f.Description.Contains(queryParams.Search));

        int totalCount = await funds.CountAsync();

        funds = funds.Skip(queryParams.PageNumber).Take(queryParams.PageSize);

        var fundsDto = funds.Select(f => new FundDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            MininumRange = f.MininumRange,
            MaximumRange = f.MaximumRange,
            ApplicationDeadline = f.ApplicationDeadline,
            NoOfApplicants = f.Applicants.Count,
            Applicants = f.Applicants.Take(2).Select(f => new ApplicantDto
            {
                ProfileImage = f.Applicant.ProfilePicture ?? ""
            }).ToList()
        });

        fundsDto = fundsDto.OrderByDescending(x => x.NoOfApplicants);
     
        var paginate = funds.Paginate(queryParams.PageNumber, queryParams.PageSize, totalCount);

        return ApiResponse<object>.Success(paginate, "Successfully retrieved funds");
    }

    public async Task<ApiResponse> GetFund(int fundId)
    {
        var fund = db.GetRepository<Fund>().GetAsync(x => x.Id == fundId).FirstOrDefaultAsync();

        if (fund == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Fund not found");

        return ApiResponse<object>.Success(fund, "Successfully retrieved fund");
    }

    public async Task<ApiResponse> GetFundApplications(int fundId, BaseQueryParams queryParams)
    {
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized");

        if (!currentUser.IsInRole(ApplicationRoles.FundProvider))
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "Action not allowed: only fund providers can view fund applications");

        var fundExists = await db.GetRepository<Fund>().GetAsync(x => x.Id == fundId && x.FunderId == currentUser.Id).FirstOrDefaultAsync();

        if (fundExists == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Fund not found");

        IQueryable<FundApplicantDto> applications = db.GetRepository<FundApplication>()
            .GetAsync(x => x.FundId == fundId)
            .Include(x => x.Applicant)
            .Select(x => new FundApplicantDto
            {
                Id = x.Applicant.Id,
                FullName = x.Applicant.FullName,
                DateApplied = x.CreatedOn,
                ProfileImage = x.Applicant.ProfilePicture ?? "",
                Status = x.Status.ToString(),
            });

        if (!string.IsNullOrEmpty(queryParams.Search))
            applications = applications.Where(f => f.FullName.Contains(queryParams.Search));

        //var fundApplicationsDto = new FundApplicationsDto
        //{
        //    Id = fundExists.Id,
        //    FundName = fundExists.Name,
        //    Description = fundExists.Description,
        //    CreatedDate = fundExists.CreatedOn,
        //    Applicants = [.. applications]
        //};

        
        int totalCount = await applications.CountAsync();

        applications = applications.OrderByDescending(x => x.DateApplied);

        applications = applications.Skip(queryParams.PageNumber).Take(queryParams.PageSize);

        var paginate = applications.Paginate(queryParams.PageNumber, queryParams.PageSize, totalCount);

        return ApiResponse<object>.Success(paginate, "Success retrieved fund applications");

    }

    public async Task<ApiResponse> RemoveFund(int fundId)
    {
        var fund = await db.GetRepository<Fund>().GetAsync(x => x.Id == fundId && x.FunderId == currentUser.Id, true).FirstOrDefaultAsync();

        if (fund == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Fund not found");

        var applicantIds = db.GetRepository<FundApplication>().GetAsync(x => x.FundId == fundId)
            .Include(x => x.Applicant)
            .Select(x => x.Applicant.Id);

        List<Notification> notifications = [];
        var notification = new Notification
        {
            Title = "Fund Removed",
            Message = $"The fund {fund.Name} has been removed by the fund provider",
            NotificationType = NotificationType.FundRemoved,
            ImageUrl = fund.ImageUrl,
            Link = $"/funds/{fund.Id}",
        };

        foreach (var userId in applicantIds)
        {
            notification.UserId = userId;
            notifications.Add(notification);
        }

        db.GetRepository<Fund>().Delete(fund);
        await db.GetRepository<Notification>().AddBulkAsync(notifications);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully removed fund");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to remove fund");
    }

    public Task<ApiResponse> UpdateFund(int fundId)
    {
        throw new NotImplementedException();
    }
}

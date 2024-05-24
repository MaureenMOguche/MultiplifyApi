using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Multiplify.Application.Constants;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos;
using Multiplify.Application.Responses;
using Multiplify.Application.ServiceImplementations.Helpers;
using Multiplify.Domain;
using Multiplify.Domain.User;

namespace Multiplify.Application.ServiceImplementations;
public class EntreprenuerService(IUnitOfWork db,
    IPhotoService photoService) : IEntreprenuerService
{
    private readonly UserPrincipal? currentUser = UserHelper.CurrentUser();

    public async Task<ApiResponse> AddUserReview(string userId, AddUserReviewDto addUserReview)
    {
        if (!currentUser.IsInRole(ApplicationRoles.Entreprenuer))
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "Action not allowed: only funders or explorers can add reviews");
        var user = db.GetRepository<AppUser>().GetAsync(x => x.Id == userId).FirstOrDefaultAsync();

        if (user == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "User not found");

        var review = new Review
        {
            Rate = addUserReview.Rate,
            Comment = addUserReview.Comment,
            ReviewerId = currentUser.Id,
            EntreprenuerId = userId
        };

        await db.GetRepository<Review>().AddAsync(review);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully added review");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to add review");
    }

    public async Task<ApiResponse> ApplyForFunding(ApplyForFundingDto applyForFunding)
    {
        if (!currentUser.IsInRole(ApplicationRoles.Entreprenuer))
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "Action not allowed: only entreprenuers can apply for funding");

        var fund = await db.GetRepository<Fund>().GetAsync(x => x.Id == applyForFunding.FundId, true).FirstOrDefaultAsync();
        if (fund == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Fund not found");

        var applicationExists = await db.GetRepository<FundApplication>().EntityExists(x => x.FundId == applyForFunding.FundId && x.ApplicantId == currentUser.Id);

        if (applicationExists)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "You have already applied for this funding");

        var application = new FundApplication
        {
            Fund = fund,
            ApplicantId = currentUser.Id,
        };

        await db.GetRepository<FundApplication>().AddAsync(application);
        if (await db.SaveChangesAsync())
            return ApiResponse.Success($"Successfully applied for {fund.Name} funding");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to apply for funding");
    }

    public async Task<ApiResponse> CreateServiceProfile(CreateServiceProfileDto createServiceProfileDto)
    {
        if (currentUser == null)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User not authenticated");

        if (!currentUser.IsInRole(ApplicationRoles.Entreprenuer))
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "Action not allowed: only entreprenuers can create service profiles");

        var serviceInfo = new ServiceInformation
        {
            EntreprenuerId = currentUser.Id,
            Title = createServiceProfileDto.Title,
            ServicesOffered = string.Join(",", createServiceProfileDto.Services),
            IsAvailable = createServiceProfileDto.IsAvailable,
            PricingType = createServiceProfileDto.PricingType,
            DeliveryTime = createServiceProfileDto.DeliveryTime,
            Link = createServiceProfileDto.Link,
        };

        if (createServiceProfileDto.ProjectImages != null)
        {
            foreach (var image in createServiceProfileDto.ProjectImages)
            {
                var upload = await photoService.AddPhotoAsync(image, $"{currentUser.Username}_{createServiceProfileDto.Title.Replace(" ", "_").ToLower()}_{Guid.NewGuid()}", "MultiplifyProjectImages");
                if (upload.Error != null)
                    return ApiResponse.Failure(StatusCodes.Status500InternalServerError, upload.Error.Message);

                serviceInfo.ProjectImages += upload.SecureUrl.AbsoluteUri + ",";
            }
        }

        await db.GetRepository<ServiceInformation>().AddAsync(serviceInfo);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully created service profile");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to create service profile");
    }

    public async Task<ApiResponse> GetAllFundings(BaseQueryParams queryParams)
    {
        var fundings = db.GetRepository<Fund>()
            .GetAsync()
            .Select(x => new
            {
                x.Name,
                x.Categories,
                x.ImageUrl,
                x.MaximumRange,
                x.MininumRange,
                x.ApplicationDeadline,
                x.Description,
                x.Requirements,
            }).AsEnumerable();

        fundings = fundings.OrderByDescending(x => x.ApplicationDeadline);

        var pagination = fundings.Paginate(queryParams.PageNumber, queryParams.PageSize);

        return ApiResponse<object>.Success(pagination, fundings.Any() ? "Successfully retrieved fundings" : "No fundings available");
    }

    public async Task<ApiResponse> GetAllServiceProfiles(BaseQueryParams queryParams)
    {
        var serviceProfiles = db.GetRepository<ServiceInformation>()
            .GetAsync().Include(x => x.Entreprenuer)
            .Select(x => new
            {
                x.EntreprenuerId,
                x.Title,
                x.Entreprenuer.Biography,
                ServicesOffered = x.ServicesOffered.Split(",", StringSplitOptions.RemoveEmptyEntries),
                x.IsAvailable,
                x.PricingType,
                x.DeliveryTime,
                ProfilePicture = x.Entreprenuer.ProfilePicture,
                x.Link
            }).AsEnumerable();

        serviceProfiles = serviceProfiles.OrderByDescending(x => x.Title);

        var pagination = serviceProfiles.Paginate(queryParams.PageNumber, queryParams.PageSize);

        return ApiResponse<object>.Success(pagination, serviceProfiles.Any() 
            ? "Successfully retrieved service profiles" : "No service profiles available");
    }

    public async Task<ApiResponse> GetBusinessProfile(string userId)
    {
        var businessProfile = await db.GetRepository<BusinessInformation>()
            .GetAsync(x => x.EntreprenuerId == userId)
            .Include(x => x.Entreprenuer)
            .Select(x => new
            {
                x.Name,
                x.Entreprenuer.Biography,
                x.Industry,
                x.Description,
                x.AverageIncome,
                x.Entreprenuer.AverageRating,
                RatingsCount = x.Entreprenuer.Reviews != null 
                    ? x.Entreprenuer.Reviews.Count() : 0,
            })
            .FirstOrDefaultAsync();

        if (businessProfile == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Business profile not found");

        return ApiResponse<object>.Success(businessProfile, "Successfully retrieved business profile");
    }

    public async Task<ApiResponse> GetServiceProfile(string userId)
    {
        var serviceInfo = await db.GetRepository<ServiceInformation>()
            .GetAsync(x => x.EntreprenuerId == userId)
            .Include(x => x.Entreprenuer)
            .ThenInclude(x => x.Reviews)
            .Select(x => new
            {
                x.Title,
                x.Entreprenuer.Biography,
                ServicesOffered = x.ServicesOffered.Split(",", StringSplitOptions.RemoveEmptyEntries),
                x.IsAvailable,
                x.PricingType,
                x.DeliveryTime,
                x.ProjectImages,
                ProfilePicture = x.Entreprenuer.ProfilePicture,
                RatingsCount = x.Entreprenuer.Reviews != null 
                    ? x.Entreprenuer.Reviews.Count() : 0,
                x.Entreprenuer.AverageRating
            })
            .FirstOrDefaultAsync();

        if (serviceInfo == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Service profile not found");

        return ApiResponse<object>.Success(serviceInfo, "Successfully retrieved service profile");
    }

    public async Task<ApiResponse> GetUserReviews(string userId)
    {
        var reviews = db.GetRepository<Review>()
            .GetAsync(x => x.EntreprenuerId == userId)
            .Include(x => x.Reviewer)
            .Select(x => new
            {
                x.Rate,
                x.Comment,
                x.Reviewer.FullName,
                x.CreatedOn
            });

        reviews = reviews.OrderByDescending(x => x.CreatedOn);

        return ApiResponse<object>.Success(reviews, reviews.Any() ? "Successfully retrieved reviews" : "No reviews available");
    }

    public async Task<ApiResponse> MyApplications(BaseQueryParams queryParams)
    {
        var applications = db.GetRepository<FundApplication>()
            .GetAsync(x => x.ApplicantId == currentUser.Id)
            .Include(x => x.Fund)
            .Select(x => new
            {
                x.Fund.Name,
                x.Fund.Categories,
                x.Fund.ImageUrl,
                x.Fund.MaximumRange,
                x.Fund.MininumRange,
                x.Status,
                LastUpdated = x.LastModifiedOn,
                DateApplied = x.CreatedOn,
            }).AsEnumerable();

        applications = applications.OrderByDescending(x => x.DateApplied);

        var pagination = applications.Paginate(queryParams.PageNumber, queryParams.PageSize);

        return ApiResponse<object>.Success(pagination, applications.Any() ? "Successfully retrieved applications" : "No applications available");
    }

    public async Task<ApiResponse> UpdateBusinessProfile(UpdateBusinessProfileDto updateBusinessProfile)
    {
        var businessInfo = await db.GetRepository<BusinessInformation>()
            .GetAsync(x => x.EntreprenuerId == currentUser.Id)
            .FirstOrDefaultAsync();

        if (businessInfo == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Business profile not found");

        businessInfo.Name = updateBusinessProfile.Name;
        businessInfo.Description = updateBusinessProfile.Description;
        businessInfo.Industry = updateBusinessProfile.Industry;
        businessInfo.AverageIncome = updateBusinessProfile.AverageIncome;
        businessInfo.Categories = string.Join(",", updateBusinessProfile.Categories);
        businessInfo.Stage = updateBusinessProfile.BusinessStage;


        db.GetRepository<BusinessInformation>().Update(businessInfo);
        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully updated business profile");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to update business profile");
    }

    public async Task<ApiResponse> UpdateServiceProfile(UpdateServiceProfileDto updateServiceProfileDto)
    {
        var serviceInfo = await db.GetRepository<ServiceInformation>()
            .GetAsync(x => x.EntreprenuerId == currentUser.Id, true)
            .FirstOrDefaultAsync();

        if (serviceInfo == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Service profile not found");

        serviceInfo.Title = updateServiceProfileDto.Title ?? serviceInfo.Title;
        serviceInfo.ServicesOffered = string.Join(",", updateServiceProfileDto.Services);
        serviceInfo.IsAvailable = updateServiceProfileDto.IsAvailable ?? serviceInfo.IsAvailable;
        serviceInfo.PricingType = updateServiceProfileDto.PricingType;
        serviceInfo.DeliveryTime = updateServiceProfileDto.DeliveryTime ?? serviceInfo.DeliveryTime;
        serviceInfo.Link = updateServiceProfileDto.Link;

        db.GetRepository<ServiceInformation>().Update(serviceInfo);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully updated service profile");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to update service profile");
    }

    public async Task<ApiResponse> UserRecommendedFunding(BaseQueryParams queryParams)
    {
        var businessInformation = await db.GetRepository<BusinessInformation>()
            .GetAsync(x => x.EntreprenuerId == currentUser.Id)
            .Select(a =>new
            {
                a.Industry,
                Categories = a.Categories.Split(",", StringSplitOptions.RemoveEmptyEntries)
            })
            .FirstOrDefaultAsync();

        var fundings = db.GetRepository<Fund>()
            .GetAsync()
            .Select(x => new
            {
                x.Name,
                x.Categories,
                x.ImageUrl,
                x.MaximumRange,
                x.MininumRange,
                x.ApplicationDeadline,
                x.Description,
                x.Requirements,
            });

        if (businessInformation != null)
        {
            var businessCategories = businessInformation.Categories
                .Distinct()
                .ToList();

            // You can access both Industry and Categories here
            var industry = businessInformation.Industry;
            businessCategories.Add(industry);

            fundings = fundings.Where(f =>
                f.Categories.Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Any(c => businessCategories.Contains(c.Trim())));

        }

        if (!string.IsNullOrEmpty(queryParams.Search))
            fundings = fundings.Where(x => x.Name.Contains(queryParams.Search));

        fundings = fundings.OrderByDescending(x => x.ApplicationDeadline);

        var pagination = fundings.Paginate(queryParams.PageNumber, queryParams.PageSize);

        return ApiResponse<object>.Success(pagination, fundings.Any() ? "Successfully retrieved recommended fundings" : "No recommended fundings available");
    }
}

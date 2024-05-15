using Multiplify.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Multiplify.Application.Dtos;
public record UpdateBusinessProfileDto(string Name,
    string Description,
    BusinessStage BusinessStage,
    string Industry,
    decimal AverageIncome,
    List<string> Categories);

public record UpdateServiceProfileDto(string? Title, 
    List<string> Services,
    bool? IsAvailable,
    ServicePricingType PricingType,
    string? DeliveryTime,
    string? Link);

public record ApplyForFundingDto(int FundId);

public record AddUserReviewDto([Range(0, 5, ErrorMessage = "Rating must be between 0 to 5")]int Rate, string? Comment);
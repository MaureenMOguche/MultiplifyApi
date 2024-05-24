using Microsoft.AspNetCore.Http;

namespace Multiplify.Application.Dtos;
public record AddFundDto(string Name, 
    string Description,
    List<string> Categories,
    string Requirements,
    decimal MininumRange,
    decimal MaximumRange,
    IFormFile? Image,
    DateTime ApplicationDeadline);


public class FundDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MininumRange { get; set; }
    public decimal MaximumRange { get; set; }
    public int NoOfApplicants { get; set; }
    public DateTime ApplicationDeadline { get; internal set; }
    public List<ApplicantDto> Applicants { get; internal set; }
}

public class ApplicantDto
{
    public string ProfileImage { get; set; } = string.Empty;
}

public record ApproveOrRejectApplicationDto(int ApplicationId, bool Approve, string? Reason);
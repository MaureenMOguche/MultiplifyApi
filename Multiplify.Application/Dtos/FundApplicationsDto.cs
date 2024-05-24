namespace Multiplify.Application.Dtos;
public class FundApplicationsDto
{
    public int Id { get; set; }
    public string FundName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public List<FundApplicantDto> Applicants { get; set; } = new();
    public string Description { get; internal set; }
}

public class FundApplicantDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ProfileImage { get; set; } = string.Empty;
    public DateTime DateApplied { get; set; }
    public string Status { get; set; } = string.Empty;
}

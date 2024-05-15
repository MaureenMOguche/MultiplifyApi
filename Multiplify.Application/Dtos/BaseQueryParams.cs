namespace Multiplify.Application.Dtos;
public class BaseQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
}

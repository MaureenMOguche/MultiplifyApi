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


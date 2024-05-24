using Microsoft.AspNetCore.Http;

namespace Multiplify.Application.Dtos.User;
public record UpdateProfileDto(string? FirsName, string? LastName, string? Biography, IFormFile ProfilePicture);


using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Multiplify.Application.ServiceImplementations.Helpers;
public static class UserHelper
{
    private static IHttpContextAccessor? _contextAccessor;

    public static void Configure(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public static UserPrincipal? CurrentUser()
    {
        if (_contextAccessor?.HttpContext?.User.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return null;

        var username = identity?.FindFirst(ClaimTypes.Name)?.Value ?? "";
        var email = identity?.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var userId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var role = identity?.FindFirst(ClaimTypes.Role)?.Value ?? "";

        return new UserPrincipal
        {
            Id = userId,
            Email = email,
            Role = role,
            Username = username
        };
    }
}

public class UserPrincipal
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public bool IsInRole(string role)
    {
        return Role == role;
    }
}

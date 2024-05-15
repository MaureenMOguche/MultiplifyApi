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

    public static UserPrincipal CurrentUser()
    {
        var identity = _contextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;

        if (identity == null || !identity.IsAuthenticated)
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
    public string Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public bool IsInRole(string role)
    {
        return Role == role;
    }
}

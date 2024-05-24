using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Responses;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Multiplify.Application.Middlewares;


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class Authorize : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authenticate = context.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme).Result;
        if (authenticate.Succeeded)
        {
            SetHttpContextUser(context, authenticate.Principal);
            return;
        }
        var response = new ApiResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Succeeded = false,
            Message = "User is not authorized"
        };

        context.Result = new ObjectResult(response);

    }

    private static void SetHttpContextUser(AuthorizationFilterContext context, ClaimsPrincipal principal)
    {
        context.HttpContext.User = principal;
    }
}

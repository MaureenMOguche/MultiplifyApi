using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Middlewares;
public static class ModelValidation
{
    public static IActionResult ValidateModel(ActionContext context)
    {
        var modelErrors = context.ModelState.AsEnumerable();
        List<string> errors = [];

        foreach (var error in modelErrors)
        {
            foreach (var err in error.Value!.Errors)
                errors.Add($"{error.Key}: {err.ErrorMessage}");
        }

        var apiResponse = ApiResponse.Failure(StatusCodes.Status400BadRequest, "One or more validation errors occured", errors);

        return new BadRequestObjectResult(apiResponse);
    }
}

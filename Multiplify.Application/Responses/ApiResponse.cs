using Microsoft.AspNetCore.Http;

namespace Multiplify.Application.Responses;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Succeeded { get; set; }

    public static ApiResponse Success(string message)
    {
        return new ApiResponse { StatusCode = StatusCodes.Status200OK, Succeeded = true, Message = message };
    }

    public static ApiResponse Failure(int statusCode, string message, List<string>? errors = null)
    {
        var errorMessages = errors != null && errors.Count == 1 ? errors.FirstOrDefault()
            : errors != null && errors.Count > 1 ? string.Join(", ", errors) : "";

        
        return new ApiResponse
        {
            StatusCode = statusCode,
            Message = string.IsNullOrEmpty(errorMessages) ? message : $"{message}, {errorMessages}",
            Succeeded = false
        };
    }

}

public class ApiResponse<T> : ApiResponse
{
    public T Data { get; set; } = default!;
    public static ApiResponse<T> Success(T data, string message)
    {
        return new ApiResponse<T> { StatusCode = StatusCodes.Status200OK, Succeeded = true, Data = data, Message = message };
    }

    public static ApiResponse<T> Failure(T data, int statusCode, string message, List<string>? errors = null)
    {
        var errorMessages = errors != null && errors.Count == 1 ? errors.FirstOrDefault()
            : errors != null && errors.Count > 1 ? string.Join(", ", errors) : "";

        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Succeeded = false,
            Data = data,
            Message = string.IsNullOrEmpty(errorMessages) ? message : $"{message}, {errorMessages}"
        };
    }
}



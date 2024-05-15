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
        var errorMessages = string.Join(", ", errors ?? Enumerable.Empty<string>());
        message = $"{message}, {errorMessages}";
        //var errorMessages = string.Join(", ", errors ?? IE)
        return new ApiResponse
        {
            StatusCode = statusCode,
            Message = message,
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
        return new ApiResponse<T> { StatusCode = statusCode, Succeeded = false, Data = data, Message = $"{message}, {string.Join(", ", errors ?? Enumerable.Empty<string>())}" };
    }
}



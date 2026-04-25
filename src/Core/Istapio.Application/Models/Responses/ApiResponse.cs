namespace Istapio.Application.Models.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int Status { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, int status = 200, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Status = status,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string error, int status = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Status = status,
            Errors = new List<string> { error }
        };
    }

    public static ApiResponse<T> ErrorResponse(List<string> errors, int status = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Status = status,
            Errors = errors
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
}

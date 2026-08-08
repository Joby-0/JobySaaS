namespace Services;
public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
    public object? Data { get; set; }

    public static ServiceResult Ok(string message, object? data = null)
    {
        return new ServiceResult
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ServiceResult Fail(string error)
    {
        return new ServiceResult
        {
            Success = false,
            Error = error
        };
    }
}
namespace BugBucks.Shared.Kernel.Responses;

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static BaseResponse Ok(string? message = null)
    {
        return new BaseResponse { Success = true, Message = message };
    }

    public static BaseResponse Fail(string message)
    {
        return new BaseResponse { Success = false, Message = message };
    }
}
namespace TaskManagement.API.Responses;

public class SuccessResponse<T> : ApiResponse<T>
{
    public SuccessResponse(T data, string message = "Success")
    {
        Success = true;
        Message = message;
        Data = data;
    }
}
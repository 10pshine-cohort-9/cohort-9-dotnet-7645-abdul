namespace TaskManagement.API.Responses;

public class ErrorResponse
{
    public bool Success => false;

    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;
}
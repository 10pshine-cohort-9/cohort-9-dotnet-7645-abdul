namespace TaskManagement.API.Responses;

public class ValidationErrorResponse : ErrorResponse
{
    public Dictionary<string, string[]> Errors { get; set; } = [];
}
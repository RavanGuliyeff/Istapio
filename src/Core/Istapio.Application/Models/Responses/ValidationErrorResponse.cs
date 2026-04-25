namespace Istapio.Application.Models.Responses;

public class ValidationErrorResponse : ErrorDetails
{
    public Dictionary<string, string[]> ValidationErrors { get; set; } = new();
}

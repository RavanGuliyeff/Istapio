namespace Istapio.Application.Models.Responses;

public class ErrorDetails
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Development only
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
}
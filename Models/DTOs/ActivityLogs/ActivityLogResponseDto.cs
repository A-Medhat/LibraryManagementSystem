namespace LibraryManagementSystem.Models.DTOs.ActivityLogs;

public class ActivityLogResponseDto
{
    public long LogId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
}

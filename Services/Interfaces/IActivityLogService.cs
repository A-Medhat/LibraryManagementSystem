using LibraryManagementSystem.Models.DTOs.ActivityLogs;

namespace LibraryManagementSystem.Services.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(string userId, string action, string entity, int? entityId, string details);
    Task<List<ActivityLogResponseDto>> GetAllAsync();
}

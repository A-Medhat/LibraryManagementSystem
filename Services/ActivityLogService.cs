using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.DTOs.ActivityLogs;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly ApplicationDbContext _context;

    public ActivityLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task LogAsync(string userId, string action, string entity, int? entityId, string details)
    {
        _context.UserActivityLogs.Add(new UserActivityLog
        {
            UserId = userId,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            Details = details,
            Timestamp = DateTime.UtcNow
        });

        return Task.CompletedTask;
    }

    public async Task<List<ActivityLogResponseDto>> GetAllAsync()
    {
        return await _context.UserActivityLogs
            .Include(x => x.User)
            .AsNoTracking()
            .OrderByDescending(x => x.Timestamp)
            .Select(x => new ActivityLogResponseDto
            {
                LogId = x.LogId,
                UserId = x.UserId,
                UserName = x.User.UserName,
                Action = x.Action,
                Entity = x.Entity,
                EntityId = x.EntityId,
                Details = x.Details,
                Timestamp = x.Timestamp
            })
            .ToListAsync();
    }
}

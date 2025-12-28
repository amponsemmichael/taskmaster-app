using TaskMaster.DTOs;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;

namespace TaskMaster.Services;

public class ActivityLogService
{
    private readonly IActivityLogRepository _repo;

    public ActivityLogService(IActivityLogRepository repo)
    {
        _repo = repo;
    }

    public async Task LogAsync(CreateActivityLogDto dto)
    {
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            TaskItemId = dto.TaskItemId,
            UserId = dto.UserId,
            Action = dto.Action,
            Description = dto.Description
        };

        await _repo.AddAsync(log);
    }

    public async Task<List<ActivityLogDto>> GetByTaskIdAsync(Guid taskId)
    {
        var logs = await _repo.GetByTaskIdAsync(taskId);

        return logs.Select(l => new ActivityLogDto
        {
            Id = l.Id,
            Action = l.Action,
            Description = l.Description,
            UserId = l.UserId,
            CreatedAt = l.CreatedAt
        }).ToList();
    }
}

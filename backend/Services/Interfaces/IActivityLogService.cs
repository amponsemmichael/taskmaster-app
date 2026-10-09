using TaskMaster.DTOs;

namespace TaskMaster.Services.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(CreateActivityLogDto dto);
    Task<List<ActivityLogDto>> GetByTaskIdAsync(Guid taskId);
}

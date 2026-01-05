using TaskMaster.Models;

namespace TaskMaster.Repositories.Interfaces;

public interface IActivityLogRepository
{
    Task AddAsync(ActivityLog log);
    Task<List<ActivityLog>> GetByTaskIdAsync(Guid taskId);
}

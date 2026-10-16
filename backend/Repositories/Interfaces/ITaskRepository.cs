using TaskMaster.Models;

namespace TaskMaster.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> GetByIdAsync(Guid id, Guid? accessibleToUserId = null);
    Task<IEnumerable<TaskItem>> GetAllAsync(Guid? accessibleToUserId = null);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> FilterAsync(
        string? status,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize,
        Guid? accessibleToUserId = null);
    Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, Guid? accessibleToUserId = null);
    Task<bool> UserCanAccessTaskAsync(Guid taskId, Guid userId);
    Task<List<TaskItem>> GetTasksDueSoonAsync(DateTime now, DateTime tomorrow);
    Task<List<TaskItem>> GetOverdueTasksAsync(DateTime now);
}

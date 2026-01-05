using TaskMaster.Models;

namespace TaskMaster.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> FilterAsync(
        string? status,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize);
    Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, Guid? userId = null);
}

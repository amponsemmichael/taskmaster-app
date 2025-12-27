using TaskMaster.Models;

namespace TaskMaster.Services.Interfaces;

public interface ITaskService
{
    Task<TaskItem> CreateAsync(TaskItem task, Guid userId);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task AssignAsync(Guid taskId, Guid userId);
    Task<IEnumerable<TaskItem>> FilterAsync(
    string? status,
    string? priority,
    DateTime? dueBefore,
    int page,
    int pageSize);      
}

using TaskMaster.Models;
using TaskMaster.DTOs.Tasks;

namespace TaskMaster.Services.Interfaces;

public interface ITaskService
{
   Task<TaskItem> CreateAsync(TaskItem task, Guid userId);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem> UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task AssignAsync(Guid taskId, Guid userId);
    Task<IEnumerable<TaskItem>> FilterAsync(
        string? status,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize);
    Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, Guid? userId = null);     
}

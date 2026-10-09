using TaskMaster.Models;
using TaskMaster.DTOs.Tasks;

namespace TaskMaster.Services.Interfaces;

public interface ITaskService
{
    Task<TaskItem> CreateAsync(CreateTaskDto dto, Guid userId);
    Task<TaskItem?> GetByIdAsync(Guid id, Guid? userId);
    Task<IEnumerable<TaskItem>> GetAllAsync(Guid? userId);
    Task<TaskItem> UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId, bool isAdmin);
    Task DeleteAsync(Guid id, Guid userId, bool isAdmin);
    Task AssignAsync(Guid taskId, Guid assignedUserId, Guid assignedByUserId);
    Task<IEnumerable<TaskItem>> FilterAsync(
        string? status,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize,
        Guid? userId);
    Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, Guid? userId);
}
